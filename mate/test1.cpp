class A
{
    int a;
    int b;
    A()
    {
        auto fff=[&](){""};
        auto f=[&](){
            int a=0;
            string b="function 254";
        }

        this->a=5;
        this->b=6;
    }
}