using System;
using System.Web.UI;

public partial class bat_dong_san_vay_mua_nha : BatDongSanPageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void btnCalc_Click(object sender, EventArgs e)
    {
        decimal value = ParseMoney(txtValue.Text);
        decimal downPercent = ParseMoney(txtDownPercent.Text);
        decimal rate = ParseMoney(txtRate.Text);
        decimal years = ParseMoney(txtYears.Text);

        if (value <= 0 || years <= 0)
        {
            lbLoanAmount.Text = lbMonthly.Text = lbTotalInterest.Text = "-";
            return;
        }

        decimal downRate = downPercent / 100m;
        decimal loanAmount = Math.Max(0, value * (1 - downRate));
        decimal monthRate = (rate / 100m) / 12m;
        int months = (int)(years * 12m);

        decimal monthly = 0;
        if (monthRate > 0)
        {
            decimal pow = (decimal)Math.Pow(1 + (double)monthRate, months);
            monthly = loanAmount * monthRate * pow / (pow - 1);
        }
        else
        {
            monthly = loanAmount / months;
        }

        decimal totalPay = monthly * months;
        decimal interest = totalPay - loanAmount;

        lbLoanAmount.Text = FormatMoney(loanAmount);
        lbMonthly.Text = FormatMoney(monthly);
        lbTotalInterest.Text = FormatMoney(interest);
    }

    private static decimal ParseMoney(string raw)
    {
        decimal v;
        if (decimal.TryParse((raw ?? "").Replace(",", "").Trim(), out v))
            return v;
        return 0;
    }

    private static string FormatMoney(decimal value)
    {
        if (value >= 1000000000m) return (value / 1000000000m).ToString("0.##") + " tỷ";
        if (value >= 1000000m) return (value / 1000000m).ToString("0.##") + " triệu";
        return value.ToString("#,##0");
    }
}
