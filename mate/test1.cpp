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
};

class IGay
{
public:
    virtual void las()=0;
};

class GayChen:B,  
 public IGay   ,A
{
    void las()
    {
        char* chen="laji Gay Chen...prpr...";
    }
};

class Fengfeng:GayChen
{

};