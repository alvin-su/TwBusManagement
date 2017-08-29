//打开加载时动画效果
function showLoading() {
    $(".loading-container").removeClass("loading-inactive")
}

//关闭加载时动画效果
function closeLoading() {
  
    $(".loading-container").addClass("loading-inactive");
}


//默认是在左上角弹出一个悬浮2000毫秒的警告框
//info:提示信息
//cssclass:弹出框的样式（sucess,error,warning,info），默认是info
//time:显示的时间,默认是2000ms
//palce:显示的位置(top_left,top_right,bottom_left,buttom_right),默认是top_right
function showNotify(info,cssclass, time, place)
{
    var icon = '';
    var type = '';
    //var _place = place;


    //console.log(place);
    if (typeof (palce) == 'undefined')
    {
        place = 'top-right';
    }

    switch (cssclass) {
        case "success":
            icon = "fa-check";
            type = "success";
            break;
        case "error":
            icon = "fa-bolt";
            type = "danger";
            break;
        case "warning":
            icon = "fa-warning";
            type = "warning";
            break;
        case "info":
            icon = "info";
            type = "fa-envelope";
            break;
        default:
            icon = "info";
            type = "fa-envelope";
            break;
    }

    if (typeof (time) == 'undefined')
    {
        time=2000
    }

    Notify(info, place, time, type, icon, true);
}

//弹出模态窗口

function ShowDialogModal(obj) {
    $("#myModalContent").html("");//
    var url = $(obj).attr("data-myurl");//弹出窗口的Url
    var title = $(obj).attr("data-mytitle");
    var css = "modal-dialog modal-lg ";
    var widthcss = $(obj).attr("data-mywidth");
    switch (widthcss) {
        case 's':
            css = 'modal-dialog modal-sm';
            break;
        case 'm':
            css = 'modal-dialog modal-lm';
            break;
        case 'l':
            css = 'modal-dialog modal-xlg';
            break;
        case 'lm':
            css = 'modal-dialog modal-lg';
            break;
        default:
            css = 'modal-dialog modal-lg';
            break
    }
    $(".modal-dialog", "#MyLargeDialogModal").attr("class", css);
   
    $(".loading-container").removeClass("loading-inactive");
    $.get(url, function (data) {
        $(".loading-container").addClass("loading-inactive");
        if (data.Statu == 'y')//session 过期
        {
            window.location.reload();
        }
        $("#mySmallModalLabel").text(title);
        $("#myModalContent").html(data);
        $('#MyLargeDialogModal').modal('show');//

     
    });
}

function CloseDialogModal(id,isHideMask)
{
    var modalid = 'MyLargeDialogModal';
    if (typeof (id) != 'undefined' && id!='')
    {
        modalid = id;
    }
    $('#' + modalid).modal('hide');
    if(isHideMask==true)
    {
        $(".modal-backdrop").hide();
    }
}

//url:弹出页面的Url
//title:弹出窗口的标题
//widthcss:弹出窗口的大小：'l'-900px;'m'-600px,'s'-300px
//弹出窗口的id
function ShowDialogModal2(url, title, widthcss, id) {
    debugger;
    var modalid = "MyLargeDialogModal";
    if (typeof (id) != 'undefined')
    {
        modalid = id;
    }
    var css = "modal-dialog modal-lg ";
    switch (widthcss) {
        case 's':
            css = 'modal-dialog modal-sm';
            break;
        case 'm':
            css = 'modal-dialog modal-lm';
            break;
        case 'l':
            css = 'modal-dialog modal-xlg';
            break;
        case 'lm':
            css = 'modal-dialog modal-lg';
            break;
        default:
            css = 'modal-dialog modal-lg';
            break
    }           
    $(".modal-dialog", $("#" + modalid)).attr("class", css);

    $(".modal-body", $("#" + modalid)).html("");// 

    $(".loading-container").removeClass("loading-inactive");
    $.get(url, function (data) {
        $(".loading-container").addClass("loading-inactive");
        if (data.Statu == 'y')//session 过期
        {
            window.location.reload();
        }
        $(".modal-title", $("#" + modalid)).text(title);
        $(".modal-body", $("#" + modalid)).html(data);
        $("#" + modalid).modal('show');
       
    });
}



function ShowDialogModal3(url, title, width) {
    var modalid = "MyLargeDialogModal";
    
    var css = "modal-dialog modal-lg ";  
    $(".modal-dialog", $("#" + modalid)).attr("class", css);

    if (typeof (width) != 'undefined') {
        $(".modal-dialog", $("#" + modalid)).attr("style", "width:" + width + "px;");
    }

    $(".modal-body", $("#" + modalid)).html("");// 

    $(".loading-container").removeClass("loading-inactive");
    $.get(url, function (data) {
        $(".loading-container").addClass("loading-inactive");
        if (data.Statu == 'y')//session 过期
        {
            window.location.reload();
        }
        $(".modal-title", $("#" + modalid)).text(title);
        $(".modal-body", $("#" + modalid)).html(data);
        $("#" + modalid).modal('show');
     
    });
}

//弹出警告框
//id为弹出警告框的id,默认为_Layout布局页的id,可传可不传:eg:'#modal-warning',PS：传过来的ID需要在前面带'#'号
//特别需要注意的是，在弹出窗口弹出警告框需要在弹出窗口界面自己定义弹出窗口的的div,在调用该函数时，将div的ID传过来；eg:AlertMsg("警告","#model")
function AlertMsg(msg, id) {
    var popformid = '#modal-warning_layout';
    if (typeof (id) != 'undefined') {
        popformid = id;
    }
    $(".modal-body", $(popformid)).html(msg);
    $(popformid).modal('show');
}

//关闭警告框
//id为弹出警告框的id,默认为_Layout布局页的id,可传可不传:eg:'#modal-warning',PS：传过来的ID需要在前面带'#'号
function CloseAlertForm(id) {
    var popformid = '#modal-warning_layout';
    if (typeof (id) != 'undefined') {
        popformid = id;
    }
    $(popformid).modal('hide');
}

function ReLoadPage()
{
    window.location.reload();
}
function replaceEmptyItem(arr){
    for(var i=0,len=arr.length;i<len;i++){
        if(!arr[i]|| arr[i]==''){
            arr.splice(i,1);
            len--;
            i--;
        }
    }
}

//设置cookie
function setCookie(name, value,time) {
    var Days = 30;
    var exp = new Date();
    if (time == undefined)
    {
        exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
    }
    else {
        exp.setTime(exp.getTime() + time);
    }
   
    document.cookie = name + "=" + escape(value) + ";path=/;expires=" + exp.toGMTString()
}
//读取cookie
function readCookie(n) {
    for (var t, r = n + "=", u = document.cookie.split(";"), i = 0; i < u.length; i++) {
        for (t = u[i]; t.charAt(0) == " ";)
            t = t.substring(1, t.length);
        if (t.indexOf(r) == 0)
            return t.substring(r.length, t.length)
    }
    return null
}
//删除cookies
function delCookie(name) {
    debugger;
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = readCookie(name);
    if (cval != null)
        document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString();
}

function Notify(n, t, i, r, u, f) {
    toastr.options.positionClass = "toast-" + t;
    toastr.options.extendedTimeOut = 0;
    toastr.options.timeOut = i;
    toastr.options.closeButton = f;
    toastr.options.iconClass = u + " toast-" + r;
    toastr.custom(n)
}
                       
