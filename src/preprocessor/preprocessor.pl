#!/usr/bin/perl

my $in;
my $out;
my $config;

my $output_file;

#Definitions hash table
my %defs;
#Macroses hash table
my %macroses;
#Area stack
my @fl_print = (1);

sub print_help {
	my $help_text = <<EOF;
-=-=TD4 Processor Developer Kit=-=-
© 2020 JL Computer Inc. All rights reserved
Author: Alexander Nekhaev
-=-=TD4 Preprocessor=-=-
Usage:
	-h -- print usage
	preprocessor file_in file_out config_file
In case of file_out missing STDOUT will be used.
Config file is necessary if #pragma CONFIG was used.
Output file will be ready for future assembly.
EOF
    print $help_text;
}

sub read_config {
    open (my $config_file, '<', $config) or die "Error opening config file \'$config\' $!";
    while (<$config_file>) {
        if (/^[ \t]*([A-Z.0-9]+)\s+(\d+)/) {
            $defs{$1} = $2;
        }
    }
    close $config_file;
}

#Reading arguments
sub identify_arguments {
	if ($ARGV[0] eq '-h') {
		print_help;
	} else {
		if ($ARGV[0]) {
			$in = $ARGV[0];
		} else {
			die "No input file specified";
		}
		if ($ARGV[1]) {
			$out = $ARGV[1];
			open ($output_file, '>', $out) or die "Error opening file \'$out\' $!";
		} else {
			$output_file = STDOUT;
		}
		if ($ARGV[2]) {
			$config = $ARGV[2];
			read_config;
		}
	}
}

sub printer {
	my $str_to_print = @_[0];
	my $printable = @_[1];
	my $replacers = join '|', keys %defs;
	$str_to_print =~ s/[, ]+($replacers)[^\w]+/$defs{$1}/;
	$str_to_print =~ s/^\s*//;
	if ($printable == 1 and $fl_print[-1]) {
		print $output_file $str_to_print;
	}
}

sub preprocess_string {
	local $str_to_preproc = @_[0];
	local $printable = 1;
	if ($str_to_preproc =~ m/^\s*$/) {
		$printable = 0;
	}
	elsif ($str_to_preproc =~ m/\s*\/\*(.*)/g) {
        push @fl_print, 0;
    }
    elsif ($str_to_preproc =~ m/\s*\*\/\s*/g) {
        pop @fl_print if @fl_print > 1;
		$printable = 0;
    }
	elsif ($str_to_preproc =~ m/(.*)\/\/.*/g) {
		preprocess_string($1);
		$printable = 0;
	}
	elsif (($str_to_preproc =~ m/^\s*#import\s*<?\"?([A-z._]+)>?\"?/) and $fl_print[-1]) {
		implement_import($1);
		$printable = 0;
	}
	elsif (($str_to_preproc =~ m/^\s*#define\s+([A-z._0-9]+)\s*([A-z._0-9]+)?/) and $fl_print[-1]) {
        if (exists($defs{$2})) {
            $defs{$1} = $defs{$2};
        } else {
            $defs{$1} = $2;
        }
        $printable = 0;
    }
	elsif ($str_to_preproc =~ m/^\s*#ifdef\s+([A-z._0-9]+)/ and $is_in_macro == 0) {
        push @fl_print, (exists($defs{$1}) and $fl_print[-1]);
        $printable = 0;
    }
    elsif ($str_to_preproc =~ m/^\s*#ifndef\s+([A-z._0-9]+)/ and $is_in_macro == 0) {
        push @fl_print, (!exists($defs{$1}) and $fl_print[-1]);
        $printable = 0;
    }
    elsif ($str_to_preproc =~ m/^\s*#else/ and $is_in_macro == 0) {
        local $last = (!$fl_print[-1]);
        pop @fl_print if @fl_print > 1;
        push @fl_print,  $last;
        $printable = 0;
    }
    elsif ($str_to_preproc =~ m/^\s*#endif/ and $is_in_macro == 0) {
        pop @fl_print if @fl_print > 1;
        $printable = 0;
    }
	elsif (($str_to_preproc =~ m/^\s*#error\s*\"?([^\"]+)\"?/) and $fl_print[-1] and $is_in_macro == 0) {
        if (exists($defs{$1})) {
			die "ERROR: $defs{$1}\n";
		} else {
			die "ERROR: $1\n";
		}
        $printable = 0;
    }
	elsif (($str_to_preproc =~ m/^\s*#message\s*\"?([^\"]+)\"?/) and $fl_print[-1] and $is_in_macro == 0) {
		if (exists($defs{$1})) {
			print STDOUT "$defs{$1}\n";
		} else {
        	print STDOUT "$1\n";
		}
        $printable = 0;
    }
	elsif (($str_to_preproc =~ m/^\s*#undef\s+([A-z._0-9]+)/) and $fl_print[-1]  and $is_in_macro == 0) {
        delete $defs{$1};
        $printable = 0;
    }
    elsif (($str_to_preproc =~ m/^\s*#sumdef\s+([A-z._0-9]+)\s([A-z._0-9]+)/) and $fl_print[-1] and $is_in_macro == 0){
        local $replacers = join '|', keys %defs;
        my $res;
        if (exists($defs{$2})) {
            $res = $defs{$2};
        } else {
            $res = $2;
        }
        $defs{$1} += $res;
        #print "# SUMDEF: $defs{$1} (stated as $2)\n";
        $printable = 0;
    }
    elsif (($str_to_preproc =~ m/^\s*#resdef\s+([A-z._0-9\-]+)\s([A-z._0-9]+)/) and $fl_print[-1] and $is_in_macro == 0){
        local $replacers = join '|', keys %defs;
        my $res;
        if (exists($defs{$2})) {
            $res = $defs{$2};
        } else {
            $res = $2;
        }
		my $result = $defs{$1} - $res;
        $defs{$1} = $result;
        #print "# RESDEF: $defs{$1} (stated as $2)\n";
        $printable = 0;
    }
	printer($str_to_preproc, $printable);
}

sub implement_import {
	my $cur_file = @_[0];
	open (my $sf, '<', $cur_file) or die "Error opening source file \'$cur_file\' $!";
	while (<$sf>) {
        my $str_to_sub = $_;
        preprocess_string($str_to_sub, $., $cur_file);
    }
    close $sf;
}

sub main {
	identify_arguments;
	implement_import($in);
	close($output_file);
}

sub print_defs {
	foreach $key (keys %defs) {
		print($key);
	}
}

main;
print_defs;