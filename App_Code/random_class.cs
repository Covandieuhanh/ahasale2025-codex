using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

public class random_class
{
    public int random_num(int min, int max)
    {
        Random random = new Random();
        return random.Next(min, max);
    }
    private string random_string(int dodai, bool lowerCase)
    {
        StringBuilder builder = new StringBuilder();
        Random random = new Random();
        char ch;
        for (int i = 0; i < dodai; i++)
        {
            ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
            builder.Append(ch);
        }
        if (lowerCase)
            return builder.ToString().ToLower();
        return builder.ToString();
    }
    public string randon_10char()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(random_string(4, true));
        builder.Append(random_num(1000, 9999));
        builder.Append(random_string(2, false));
        return builder.ToString();
    }

    public string radndom_num_str(int doadai)
    {
        string allChar = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
        string[] allCharArray = allChar.Split(',');
        string randomCode = "";
        int temp = -1;

        Random rand = new Random();
        for (int i = 0; i < doadai; i++)
        {
            if (temp != -1)
            {
                rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
            }
            int t = rand.Next(allCharArray.Length);
            if (temp != -1 && temp == t)
            {
                return radndom_num_str(doadai);
            }
            temp = t;
            randomCode += allCharArray[t];
        }
        return randomCode;
    }
    public string random_number(int _length)
    {
        string allChar = "0,1,2,3,4,5,6,7,8,9";
        string[] allCharArray = allChar.Split(',');
        string randomCode = "";
        int temp = -1;

        Random rand = new Random();
        for (int i = 0; i < _length; i++)
        {
            if (temp != -1)
            {
                rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
            }
            int t = rand.Next(allCharArray.Length);
            if (temp != -1 && temp == t)
            {
                return random_number(_length);
            }
            temp = t;
            randomCode += allCharArray[t];
        }
        return randomCode;
    }
}