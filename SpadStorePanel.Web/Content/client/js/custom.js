// JavaScript Document
$(function () {
    var showBuy = false;
    var showAccount = false;
    $('#gift').click(function(){
        if($(this).prop("checked") == true){
            $(".gift-option").css("display","block")
        }
        else if($(this).prop("checked") == false){
            $(".gift-option").css("display","none")
        }
    });
    $('#postal-card').click(function(){
        if($(this).prop("checked") == true){
            $(".postal").css("display","block")
        }
        else if($(this).prop("checked") == false){
            $(".postal").css("display","none")
        }
    });

    $('.buy').on("click", function () {
        if (showBuy) {
            $(".cart-dropdown").css("display", "none");
            showBuy = false;
        } else {
            $(".cart-dropdown").css("display", "block");
            showBuy = true;
        }

    });
    $('.account').on("click", function () {
        if (showAccount) {
            $(".account-link-dropdown").css("display", "none");
            showAccount = false;
        } else {
            $(".account-link-dropdown").css("display", "block");
            showAccount = true;
        }

    });

    $('img').mousedown(function (e) {
        if (e.button == 2) { // right click
            return false; // do nothing!
        }
    });
    "use strict";

    // index : header animation*/
     $(window).on("scroll", function () {

         initViewPositioning();
     });

    $(document).ready(function () {
        initViewPositioning();
    })

    function initViewPositioning() {
        if ($(this).scrollTop() > 94) {
            $("header").addClass("header-fixed");
            $(".header-top-row").addClass("hidden-logo");
            $(".onscroll-logo-container").addClass("active-onscroll-logo-container");
            $(".mobile-search").addClass("hidden");
        } else {
            $("header").removeClass("header-fixed");
            $(".header-top-row").removeClass("hidden-logo");
            $(".onscroll-logo-container").removeClass("active-onscroll-logo-container");
            $(".mobile-search").removeClass("hidden");
        }
    }

    /*end header animation*/

    function responsive_dropdown() {

        /* ---- For Mobile Menu Dropdown JS Start ---- */
        $('#menu span.opener').on("click", function () {
            if ($(this).hasClass("plus")) {
                $(this).parent().find('.mobile-sub-menu').slideDown();
                $(this).removeClass('plus');
                $(this).addClass('minus');
            } else {
                $(this).parent().find('.mobile-sub-menu').slideUp();
                $(this).removeClass('minus');
                $(this).addClass('plus');
            }
            return false;
        });
        /* ---- For Mobile Menu Dropdown JS End ---- */
        /* ---- For Footer JS Start ---- */
        $('.footer-static-block .head-three, .footer-static-block span.opener').on("click", function () {
            if ($(this).parent('.footer-static-block').hasClass("active")) {
                if ($(window).width() < 768) {
                    $(this).parent().find('.footer-block-contant').slideUp();
                    $(this).parent('.footer-static-block').removeClass('active');
                    $(this).parent().find('span.opener').addClass('plus');
                    $(this).parent().find('span.opener').removeClass('minus');
                }
            } else {
                if ($(window).width() < 768) {
                    $(this).parent().find('.footer-block-contant').slideDown();
                    $(this).parent('.footer-static-block').addClass('active');
                    $('.footer-static-block.active span.opener').addClass('minus');
                    $('.footer-static-block.active span.opener').removeClass('plus');
                }
            }
            return false;
        });
        /* ---- For Footer JS End ---- */

        /* ---- For Navbar JS Start ---- */
        $('.navbar-toggle').on("click", function () {
            var menu_id = $('#menu');
            var nav_icon = $('.navbar-toggle i');
            if (menu_id.hasClass('menu-open')) {
                menu_id.removeClass('menu-open');
                nav_icon.removeClass('fa-close');
                nav_icon.addClass('fa-bars');
            } else {
                menu_id.addClass('menu-open');
                nav_icon.addClass('fa-close');
                nav_icon.removeClass('fa-bars');
            }
            return false;
        });
        /* ---- For Navbar JS End ---- */


        //***************************************** search popup section ******************************************///
        /*$('li.search-box').on('click', function () {
            $('li.search-box').toggleClass("active-search-box");
            $('.search-section').toggleClass("active-search-section");
            //$('.sidebar-search-wrap').addClass('open').siblings().removeClass('open');
            return false;
        });*/
        $(".search-popup-btn").on('click', function () {
            $('li.search-box').toggleClass("active-search-box");
            $('.search-section').toggleClass("active-search-section");
            //$('.sidebar-search-wrap').addClass('open').siblings().removeClass('open');
            return false;
        })

        /*Search-box-close-button*/

        $('.search-closer').on('click', function () {
            var sidebar = $('.sidebar-navigation');
            var nav_icon = $('.navbar-toggle i');
            if (sidebar.hasClass('open')) {
                //sidebar.removeClass('open');
            } else {
                sidebar.addClass('open');
                nav_icon.addClass('fa-search-close');
                nav_icon.removeClass('fa-search-bars');
            }

            $('.sidebar-search-wrap').removeClass('open');
            return false;
        });

        $('.sidebar-search-wrap').on('click', function () {
            var sidebar = $('.sidebar-navigation');
            var nav_icon = $('.navbar-toggle i');
            if (sidebar.hasClass('open')) {
                //sidebar.removeClass('open');
            } else {
                sidebar.addClass('open');
                nav_icon.addClass('fa-search-close');
                nav_icon.removeClass('fa-search-bars');
            }

            $('.sidebar-search-wrap').removeClass('open');
            return false;
        })

        $('.sidebar-table-container').on('click', function (e) {
            e.stopPropagation();
            e.preventDefault();
        })

    }

    /* owl slider */
    if ($(".brand-slider").length > 0) {
        $(".brand-slider").owlCarousel({
            items: 1,
            autoplay: true,
            autoPlaySpeed: 5000,
            autoPlayTimeout: 5000,
            autoplayHoverPause: true,
            rtl: true,
            dots: false,
            nav: false,
            loop: true,
            responsiveClass: true,
            responsive: {
                0: {
                    items: 2,
                },
                575: {
                    items: 3,
                },
                767: {
                    items: 4,
                },
                1200: {
                    items: 6,
                },
            }
        });
    }

    if ($(".main-banner, .testimonial-slider").length > 0) {
        $(".main-banner, .testimonial-slider").owlCarousel({
            items: 1,
            autoplay: true,
            autoPlaySpeed: 5000,
            autoPlayTimeout: 5000,
            autoplayHoverPause: true,
            rtl: true,
            loop: true,
            autoplay: true,
            dots: false,
            nav: false,
            loop: true,
            responsiveClass: true,
            responsive: {
                767: {
                    dots: false,
                    nav: true,
                },
            }
        });
    }

    if ($(".product-slider").length > 0) {
        $(".product-slider").owlCarousel({
            items: 1,
            autoplay: true,
            autoPlaySpeed: 5000,
            autoPlayTimeout: 5000,
            autoplayHoverPause: true,
            rtl: true,
            loop: true,
            autoplay: true,
            dots: false,
            nav: false,
            loop: true,
            responsiveClass: true,
            nav: true,
            responsive: {
                0: {
                    items: 2,
                },
                420: {
                    items: 2,
                },
                767: {
                    items: 3,
                },
                991: {
                    items: 4,
                },
            }
        });
    }

    /*end owl slider*/

    function product_page_tab() {
        $("#tabs li a").on("click", function (e) {
            var title = $(e.currentTarget).attr("title");
            $("#tabs li a , .tab_content li div").removeClass("selected");
            $(".tab-" + title + ", .items-" + title).addClass("selected");
            $("#items").attr("class", "tab-" + title);

            return false;
        });
    }

    /* menu overlay start */
    $(".navbar-toggle").on("click", function () {
        if (!$(".navbar-collapse").hasClass("menu-open")) {
            $(".overlay").fadeIn("slow")
        }
    })
    $(".overlay").on("click", function () {

        $(this).fadeOut();
        $(".navbar-collapse").removeClass("in").addClass("collapse");
        $(".navbar-toggle").click();
        return false;
    })

    $('.nav-link').on('click', function (e) {
        $('a.nav-link').removeClass('active');
        $(this).addClass('active');
        if ($(window).width() < 991) {
            $(".navbar-toggle").click();
            $(".overlay").fadeOut();
            $(".navbar-collapse").removeClass("in").addClass("collapse");
        }
    });
    /* menu overlay end */


    $(document).ready(function () {
        responsive_dropdown();
        product_page_tab();
    });
    $('.add-to-cart a[href="#"]').click(function (event) {
           
        event.preventDefault();
});

});






/* Multiple Timer Section Start */

var doc = document;

function queryElm(elm, all) {
    if (all)
        return doc.querySelectorAll(elm);
    else
        return doc.querySelector(elm);

}


function CountDown(elm) {
    var now = new Date();

    var strDeadline = elm.getAttribute("m-date-date");
    var deadline = ParseToDate(strDeadline);

    var timespan = GetTimeSpan(now, deadline);

    var strTimerVal = '';
    // always show days-hours-minutes-seconds
    var strSeconds = timespan[5] >= 10 ? timespan[5] : "0" + timespan[5];
    strSeconds = getArabicNumbers(strSeconds);
    strTimerVal += '<div class="time-part">' + strSeconds + '</div>';

    var strminutes = timespan[4] >= 10 ? timespan[4] : "0" + timespan[4];
    strminutes = getArabicNumbers(strminutes);
    strTimerVal += '<div class="time-part">' + strminutes + '</div>';

    var strHours = timespan[3] >= 10 ? timespan[3] : "0" + timespan[3];
    strHours = getArabicNumbers(strHours);
    strTimerVal += '<div class="time-part">' + strHours + '</div>';

    var strDays = timespan[2] >= 10 ? timespan[2] : "0" + timespan[2];
    strDays = getArabicNumbers(strDays);
    strTimerVal += '<div class="time-part">' + strDays + '</div>';

    if (timespan[1] > 0) {
        var strmonths = timespan[1] >= 10 ? timespan[1] : "0" + timespan[1];
        strmonths = getArabicNumbers(strmonths);
        strTimerVal += '<div class="time-part">' + strmonths + '</div>';
    }
    if (timespan[0] > 0) {
        var stryears = timespan[0] >= 10 ? timespan[0] : "0" + timespan[0];
        stryears = getArabicNumbers(stryears);
        strTimerVal = '<div class="time-part">' + stryears + '</div>';
    }

    elm.innerHTML = strTimerVal;

}


function StartTimer() {
    var objects = queryElm(".timer-secton", true);
    if (objects.length < 1)
        return;
    objects.forEach(CountDown);
}


function ParseToDate(strDate) {
    var datePart = strDate.split(" ")[0];
    var timePart = strDate.split(" ")[1];


    var dateParts = datePart.split("/");
    var timeParts = timePart.split(":");


    var year = dateParts[0];
    var month = parseInt(dateParts[1]) - 1;// javascript counts months from 0-11
    var day = dateParts[2];
    var hour = timeParts[0];
    var minute = timeParts[1];
    var second = timeParts[2];

    var date = new Date(year, month, day, hour, minute, second);


    return date;

}

function GetTimeSpan(startDate, endDate) {
    var TimeSpan = [0, 0, 0, 0, 0, 0];
    if (startDate > endDate)
        return TimeSpan;


    var diff = endDate - startDate; // value in milliseconds

    var diffInSeconds = Math.round(diff / 1000); // time span in seconds


    var numOfYears = Math.trunc(diffInSeconds / (60 * 60 * 24 * 365));
    TimeSpan[0] = numOfYears;
    diffInSeconds = diffInSeconds % (60 * 60 * 24 * 365);

    var numOfMonths = Math.trunc(diffInSeconds / (60 * 60 * 24 * 30));
    TimeSpan[1] = numOfMonths;
    diffInSeconds = diffInSeconds % (60 * 60 * 24 * 30);

    var numOfDays = Math.trunc(diffInSeconds / (60 * 60 * 24));
    TimeSpan[2] = numOfDays;
    diffInSeconds = diffInSeconds % (60 * 60 * 24);

    var numOfHours = Math.trunc(diffInSeconds / (60 * 60));
    TimeSpan[3] = numOfHours;
    diffInSeconds = diffInSeconds % (60 * 60);

    var numOfmitute = Math.trunc(diffInSeconds / (60));
    TimeSpan[4] = numOfmitute;
    diffInSeconds = diffInSeconds % (60);

    var numOfSeconds = Math.trunc(diffInSeconds);
    TimeSpan[5] = numOfSeconds;


    return TimeSpan;
}

var map =
    [
        "&\#1632;", "&\#1633;", "&\#1634;", "&\#1635;", "&\#1636;",
        "&\#1637;", "&\#1638;", "&\#1639;", "&\#1640;", "&\#1641;"
    ];

function getArabicNumbers(str) {
    var newStr = "";

    str = String(str);

    for (i = 0; i < str.length; i++) {
        newStr += map[parseInt(str.charAt(i))];
    }

    return newStr;
}


$(document).ready(function () {
    setInterval(() => {
        StartTimer();
    }, 1000);


    // configure toastr

    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-top-right",//"toast-top-full-width",//
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
})


/* Multiple Time Section End */




function CopyLink(text) {
    if (window.clipboardData && window.clipboardData.setData) {
        // Internet Explorer-specific code path to prevent textarea being shown while dialog is visible.
        return clipboardData.setData("Text", text);

    }
    else if (document.queryCommandSupported && document.queryCommandSupported("copy")) {
        var textarea = document.createElement("textarea");
        textarea.textContent = text;
        textarea.style.position = "fixed";  // Prevent scrolling to bottom of page in Microsoft Edge.
        document.body.appendChild(textarea);
        textarea.select();
        try {
            document.execCommand("copy");  // Security exception may be thrown by some browsers.
            toastr.info("لینک محصول کپی شد");
            return true;
        }
        catch (ex) {
            console.warn("Copy to clipboard failed.", ex);
            return false;
        }
        finally {
            document.body.removeChild(textarea);
        }
    }
}







$(window).on("load", function () {
    "use strict";
    /* -------- preloader ------- */
    $('#preloader').delay(100).fadeOut(500);
    /*------End----------*/

});























