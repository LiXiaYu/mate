# mate
Mate, a new language, which is translated to cpp

# How to use it
## Translator.cs
### the main translator program

## Program.cs
### only Main()
#### *test1.mate* to *test1.cpp*

# Language Feature
## mate
### **mate** replace to **class**
#### like:
mate Fengfeng
{

};
##### to
class Fengfeng
{

};

## this
### **this.** replace to **this->**
#### like:
this.a=5;
this.b=6;
##### to
this->a=5;
this->b=6;

## lambda
### **function** replace to **[&]**
#### like:
auto f=function(){
    int a=0;
    string b="function 254";
};
##### to
auto f=[&](){
    int a=0;
    string b="function 254";
};

## interface
### **interface** replace to **class** with virtual func()=0
### **interface** inherit just as public inherit
#### like:
interface IGay
{
    void las();
};

mate GayChen:B,  
 IGay   ,A
{
    void las()
    {
        char* chen="laji Gay Chen...prpr...";
    }
};
##### to
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
