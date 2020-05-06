#include <stdio.h>
#include <stdlib.h>
#include <string.h>

typedef struct label_rec{
    long long label_name;
    int string_num;
    struct label_rec* next;
} label_rec;

label_rec* root;

long long compute_hash(char* s) {
    const int p = 31;
    const int m = 1e9 + 9;
    long long hash_value = 0;
    long long p_pow = 1;
    for (int i = 0; i < strlen(s); i++) {
        char c = s[i];
        hash_value = (hash_value + (c - 'A' + 1) * p_pow) % m;
        p_pow = (p_pow * p) % m;
    }
    return hash_value;
}

void add_label(char* label_name, char* arg1) {
    label_rec* finder = root;
    while (finder->next != NULL)
    {
        finder = root->next;
    }
    label_rec* new_label = (label_rec*)malloc(sizeof(label_rec));
    new_label->label_name = compute_hash(label_name);
    new_label->string_num = atoi(arg1);
    new_label->next = NULL;
    finder->next = new_label;
}

void init() {
    root = (label_rec*)malloc(sizeof(label_rec));
    root->label_name = 0;
    root->string_num = 0;
    root->next = NULL;
}

int find_label(char* label_name) {
    label_rec* finder = root;

    //Check if label is number
    if (atoi(label_name) > 0 || label_name[0] == '0') {
        return atoi(label_name);
    }

    //Look up label
    while (finder != NULL)
    {
        if (finder->label_name == compute_hash(label_name)) {
            return finder->string_num;
        }
        finder = finder->next;
    }

    //Return promise or error in future
    return -1;
}

int find_label_by_hash(long long label_name) {
    label_rec* finder = root;

    //Look up label
    while (finder != NULL)
    {
        if (finder->label_name == label_name) {
            return finder->string_num;
        }
        finder = finder->next;
    }

    //Error
    return -1;
}

void print_dict() {
    label_rec* runner = root;
    while (runner != NULL)
    {
        printf("%lld -> %d\n", runner->label_name, runner->string_num);
        runner = runner->next;
    }
    
}