using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class thongbao_class
{
    public static string metro_notifi(string _tieude, string _noidung, string _timeout, string _class)
    {
        return "var notify = Metro.notify;notify.setup({timeout: " + _timeout + ",});notify.create('"
            + _noidung + "', '" + _tieude + "', {cls: '" + _class + "'});";
    }
    public static string metro_dialog(string _tieude, string _noidung, string _closebutton, string _overlayClickClose, string _butname1, string _class1, string _link1)
    {
        string _but1 = "", _l1 = "";
        if (_link1 != "")
            _l1 = ",onclick: function(){window.location = '" + _link1 + "';}";
        _but1 = "{caption: '" + _butname1 + "',cls: 'js-dialog-close " + _class1 + "'" + _l1 + "},";
        string _dialog = "Metro.dialog.create({title: '" + _tieude + "',content: '<div>" + _noidung + "</div>',closeButton: " + _closebutton + ",overlayClickClose: " + _overlayClickClose
            + ",actions: [" + _but1 + "]});";
        return _dialog;
    }
    public static string metro_dialog_2but(string _tieude, string _noidung, string _closebutton, string _overlayClickClose, string _butname1, string _class1, string _link1, string _butname2, string _class2, string _link2)
    {
        string _but1 = "", _l1 = "";
        if (_link1 != "")
            _l1 = ",onclick: function(){window.location = '" + _link1 + "';}";
        _but1 = "{caption: '" + _butname1 + "',cls: 'js-dialog-close " + _class1 + "'" + _l1 + "},";

        string _but2 = "", _l2 = "";
        if (_link2 != "")
            _l2 = ",onclick: function(){window.location = '" + _link2 + "';}";
        _but2 = "{caption: '" + _butname2 + "',cls: 'js-dialog-close " + _class2 + "'" + _l2 + "},";

        string _dialog = "Metro.dialog.create({title: '" + _tieude + "',content: '<div>" + _noidung + "</div>',closeButton: " + _closebutton + ",overlayClickClose: " + _overlayClickClose
            + ",actions: [" + _but1 + _but2 + "]});";
        return _dialog;
    }

    public static string metro_notifi_onload(string _tieude, string _noidung, string _timeout, string _class)
    {
        return "<script>function openNotifi() {var notify = Metro.notify;notify.setup({timeout: " + _timeout + ",});notify.create('"
            + _noidung + "', '" + _tieude + "', {cls: '" + _class + "'});}window.onload = function () {openNotifi();};</script>";
    }
    public static string metro_dialog_onload(string _tieude, string _noidung, string _closebutton, string _overlayClickClose, string _butname1, string _class1, string _link1)
    {
        string _but1 = "", _l1 = "";
        if (_link1 != "")
            _l1 = ",onclick: function(){window.location = '" + _link1 + "';}";
        _but1 = "{caption: '" + _butname1 + "',cls: 'js-dialog-close " + _class1 + "'" + _l1 + "},";
        string _dialog = "<script>function openDialog() {Metro.dialog.create({title: '" + _tieude + "',content: '<div>" + _noidung + "</div>',closeButton: " + _closebutton + ",overlayClickClose: " + _overlayClickClose
            + ",actions: [" + _but1 + "]});}window.onload = function () {openDialog();};</script>";
        return _dialog;
    }
    public static string metro_dialog_2but_onload(string _tieude, string _noidung, string _closebutton, string _overlayClickClose, string _butname1, string _class1, string _link1, string _butname2, string _class2, string _link2)
    {
        string _but1 = "", _l1 = "";
        if (_link1 != "")
            _l1 = ",onclick: function(){window.location = '" + _link1 + "';}";
        _but1 = "{caption: '" + _butname1 + "',cls: 'js-dialog-close " + _class1 + "'" + _l1 + "},";

        string _but2 = "", _l2 = "";
        if (_link2 != "")
            _l2 = ",onclick: function(){window.location = '" + _link2 + "';}";
        _but2 = "{caption: '" + _butname2 + "',cls: 'js-dialog-close " + _class2 + "'" + _l2 + "},";

        string _dialog = "<script>function openDialog() {Metro.dialog.create({title: '" + _tieude + "',content: '<div>" + _noidung + "</div>',closeButton: " + _closebutton + ",overlayClickClose: " + _overlayClickClose
            + ",actions: [" + _but1 + _but2 + "]});}window.onload = function () {openDialog();};</script>";
        return _dialog;
    }
}