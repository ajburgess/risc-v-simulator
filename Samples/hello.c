short x = 0x55;
char *terminal = (char *)0x7000;

void print(char *text)
{
    char c;
    while (1)
    {
        char c = *text;
        if (c == 0x00)
            break;
        *terminal = c;
        text++;
    }
}

void main()
{
    char *msg = "Hello!\n\0";
    print(msg);
}