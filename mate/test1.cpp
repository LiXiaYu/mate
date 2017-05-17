class A
{
    int a;
    int b;
    A()
    {
        auto f=[&](){
            int a=0;
        }

        this->a=5;
        this->b=6;
    }
}