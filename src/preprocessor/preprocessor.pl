#!/usr/bin/perl

my $in;
my $out;
my $config;
my $big_comm;

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
© 2018 JL Computer Inc. All rights reserved
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
	if ($printable == 1 and $big_comm == 0) {
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
        $big_comm = 1;
    }
    elsif ($str_to_preproc =~ m/\s*\*\/\s*/g) {
        $big_comm = 0;
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
            #my @def_keys = keys %defs;
            #print "# DEF\[$str_file_name\:$str_number\]: $1->$defs{$1}\nCurrent keys: @def_keys\n";
            $printable = 0;
        }
	printer($str_to_preproc, $printable);
}

sub implement_import {
	my $cur_file = @_[0];
	open (my $sf, '<', $cur_file) or die "Error opening source file \'$cur_file\' $!";
	while (<$sf>) {
        my $str_to_sub = $_;
        preproc_string($str_to_sub, $., $cur_file);
    }
    close $sf;
}

sub main {
	identify_arguments;
	implement_import($in);
	close($output_file);
}

main;