using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;


public class number_class
{
    public static string number_to_text_limit(decimal total)  //đọc đc 18 số
    {
        try
        {
            string rs = "";
            total = Math.Round(total, 0);
            string[] ch = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] rch = { "lẻ", "mốt", "", "", "", "lăm" };
            string[] u = { "", "mươi", "trăm", "ngàn", "", "", "triệu", "", "", "tỷ", "", "", "ngàn", "", "", "triệu" };
            string nstr = total.ToString();

            int[] n = new int[nstr.Length];
            int len = n.Length;
            for (int i = 0; i < len; i++)
            {
                n[len - 1 - i] = Convert.ToInt32(nstr.Substring(i, 1));
            }

            for (int i = len - 1; i >= 0; i--)
            {
                if (i % 3 == 2)// số 0 ở hàng trăm
                {
                    if (n[i] == 0 && n[i - 1] == 0 && n[i - 2] == 0) continue;//nếu cả 3 số là 0 thì bỏ qua không đọc
                }
                else if (i % 3 == 1) // số ở hàng chục
                {
                    if (n[i] == 0)
                    {
                        if (n[i - 1] == 0) { continue; }// nếu hàng chục và hàng đơn vị đều là 0 thì bỏ qua.
                        else
                        {
                            rs += " " + rch[n[i]]; continue;// hàng chục là 0 thì bỏ qua, đọc số hàng đơn vị
                        }
                    }
                    if (n[i] == 1)//nếu số hàng chục là 1 thì đọc là mười
                    {
                        rs += " mười"; continue;
                    }
                }
                else if (i != len - 1)// số ở hàng đơn vị (không phải là số đầu tiên)
                {
                    if (n[i] == 0)// số hàng đơn vị là 0 thì chỉ đọc đơn vị
                    {
                        if (i + 2 <= len - 1 && n[i + 2] == 0 && n[i + 1] == 0) continue;
                        rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);
                        continue;
                    }
                    if (n[i] == 1)// nếu là 1 thì tùy vào số hàng chục mà đọc: 0,1: một / còn lại: mốt
                    {
                        rs += " " + ((n[i + 1] == 1 || n[i + 1] == 0) ? ch[n[i]] : rch[n[i]]);
                        rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);
                        continue;
                    }
                    if (n[i] == 5) // cách đọc số 5
                    {
                        if (n[i + 1] != 0) //nếu số hàng chục khác 0 thì đọc số 5 là lăm
                        {
                            rs += " " + rch[n[i]];// đọc số 
                            rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);// đọc đơn vị
                            continue;
                        }
                    }
                }

                rs += (rs == "" ? " " : ", ") + ch[n[i]];// đọc số
                rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);// đọc đơn vị
            }
            if (rs[rs.Length - 1] != ' ')
                rs += " đồng.";
            else
                rs += "đồng.";

            if (rs.Length > 2)
            {
                string rs1 = rs.Substring(0, 2);
                rs1 = rs1.ToUpper();
                rs = rs.Substring(2);
                rs = rs1 + rs;
            }
            return rs.Trim().Replace("lẻ,", "lẻ").Replace("mươi,", "mươi").Replace("trăm,", "trăm").Replace("mười,", "mười");
        }
        catch
        {
            return "Số bạn nhập vào quá lớn";
        }
    }

    public static string number_to_text_unlimit(string number)
    {
        string[] dv = { "", "mươi", "trăm", "nghìn", "triệu", "tỉ" };
        string[] cs = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
        string doc;
        int i, j, k, n, len, found, ddv, rd;

        len = number.Length;
        number += "ss";
        doc = "";
        found = 0;
        ddv = 0;
        rd = 0;

        i = 0;
        while (i < len)
        {
            //So chu so o hang dang duyet
            n = (len - i + 2) % 3 + 1;

            //Kiem tra so 0
            found = 0;
            for (j = 0; j < n; j++)
            {
                if (number[i + j] != '0')
                {
                    found = 1;
                    break;
                }
            }

            //Duyet n chu so
            if (found == 1)
            {
                rd = 1;
                for (j = 0; j < n; j++)
                {
                    ddv = 1;
                    switch (number[i + j])
                    {
                        case '0':
                            if (n - j == 3) doc += cs[0] + " ";
                            if (n - j == 2)
                            {
                                if (number[i + j + 1] != '0') doc += "lẻ ";
                                ddv = 0;
                            }
                            break;
                        case '1':
                            if (n - j == 3) doc += cs[1] + " ";
                            if (n - j == 2)
                            {
                                doc += "mười ";
                                ddv = 0;
                            }
                            if (n - j == 1)
                            {
                                if (i + j == 0) k = 0;
                                else k = i + j - 1;

                                if (number[k] != '1' && number[k] != '0')
                                    doc += "mốt ";
                                else
                                    doc += cs[1] + " ";
                            }
                            break;
                        case '5':
                            if (i + j == len - 1)
                                doc += "lăm ";
                            else
                                doc += cs[5] + " ";
                            break;
                        default:
                            doc += cs[(int)number[i + j] - 48] + " ";
                            break;
                    }

                    //Doc don vi nho
                    if (ddv == 1)
                    {
                        doc += dv[n - j - 1] + " ";
                    }
                }
            }


            //Doc don vi lon
            if (len - i - n > 0)
            {
                if ((len - i - n) % 9 == 0)
                {
                    if (rd == 1)
                        for (k = 0; k < (len - i - n) / 9; k++)
                            doc += "tỉ ";
                    rd = 0;
                }
                else
                    if (found != 0) doc += dv[((len - i - n + 1) % 9) / 3 + 2] + " ";
            }

            i += n;
        }

        if (len == 1)
            if (number[0] == '0' || number[0] == '5') return cs[(int)number[0] - 48];


        string rs1 = doc.Substring(0, 1);
        rs1 = rs1.ToUpper();
        doc = doc.Substring(1, doc.Length - 1);
        doc = rs1 + doc;

        return doc;
    }
    public bool check_int(string _str)
    {
        foreach (Char c in _str)
        {
            if (!Char.IsDigit(c))
                return false;
        }
        return true;
    }
    public bool check_float(string _num)
    {
        Regex regex = new Regex(@"^[-+]?[0-9]*\.?[0-9]+$");
        return regex.IsMatch(_num);
    }
    public string trave_dinhdangtien(string _str)
    {
        string _a = int.Parse(_str).ToString("#,#");
        return _a == "" ? "0" : _a;
    }
    public string random_bcorn(int _num)
    {
        Random _ra = new Random();
        return _ra.Next(0, 2).ToString();
    }
}