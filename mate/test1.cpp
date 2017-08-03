/*Good Mate is a good class*/class A
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

//Laji GayChen interface
//mate B
//{
//};
//
//
//
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
//hehe
class IPa
{
public:
	virtual void papa()=0;
};

class Mim:public IGay,public IPa
{
};
/*dssfsa*/
class Lou:public IPa,public IGay
{};
