using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class number_of_page_class
{
    public static int return_total_page(int _sum_of_page, int _number_of_display)
    {
        if (_sum_of_page != 0)
        {
            if (_sum_of_page % _number_of_display == 0)
                return (_sum_of_page / _number_of_display);
            else
                return (_sum_of_page / _number_of_display) + 1;
        }
        else
            return 1;
    }
}