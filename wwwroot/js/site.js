//******************************************************************************
//
//  HEADER NAV                                                      HEADER NAV
//
//*******************************************************************************
var $window = $(window);
var $button = document.querySelector("#hamburger-toggle");
var $mainWrapper = document.querySelector("#main-wrapper");
var $brand = document.querySelector('.header-brand')
var $sidebar = document.querySelector('#sidebar-wrapper');

// *****  for showing and hiding side nav and toggling hamburger icon *****
$button.addEventListener("click", (e) => {
    $('#main-wrapper').addClass('no-pointer'); // disable mouse over event listener
    $mainWrapper.classList.toggle("toggled"); // toggle main-wrapper
    $brand.classList.toggle("toggled"); // toggle header-container div
    $button.classList.toggle('changed'); // toggle hamburger/x icon
    $("#main-wrapper").on('transitionend webkitTransitionEnd oTransitionEnd otransitionend MSTransitionEnd',
        function () {
            // re-enable mouseEnter event when animation completes.
            // adding a little extra time
            setTimeout(function () {
                $('#main-wrapper').removeClass('no-pointer');
            }, 100);
        });
});

// *****  expands sidebar when mouse over  *****
$sidebar.addEventListener("mouseenter", () => {
    if ($("#main-wrapper").css('padding-left') == 250 + 'px' || $(window).width() < 768) {
        return false;
    } else if ($("#main-wrapper").css('padding-left') == 60 + 'px' && $("#main-wrapper").hasClass("toggled")) {
        $("#main-wrapper").removeClass("toggled"); // toggle main-wrapper
        $('.header-brand').removeClass("toggled"); // toggle header-container div
        $("#hamburger-toggle").removeClass('changed'); // toggle hamburger/x icon
    } else {
        $mainWrapper.classList.toggle("toggled"); // toggle main-wrapper
        $brand.classList.toggle("toggled"); // toggle header-container div
        $button.classList.toggle('changed'); // toggle hamburger/x icon
    }
});

// *****  removes/resets toggle & hamburger on window resize 768  ******
$window.on("resize", function () {
    $("#main-wrapper").removeClass("toggled"); // toggle main-wrapper
    $('.header-brand').removeClass("toggled"); // toggle header-container div
    $("#hamburger-toggle").removeClass('changed'); // toggle hamburger/x icon
});

// *****  accordian menu expand/collapse  ******
$("li.dropdown").click(function () {
    $("li.dropdown").not(this).find("ul").hide(400);
    $("li.dropdown").not(this).find(".iChev").removeClass("bi-chevron-down")
        .addClass("bi-chevron-right");
    $(this).find("ul").toggle(400);
    $(this).find(".iChev").toggleClass("bi-chevron-right bi-chevron-down");
    // prevent the menu from collapsing when clicking on a sub menu item
    $("li.dropdown ul li a").click(function (e) {
        e.stopPropagation();
    });
});

// ****  applies active class to clicked link  ****
$(function () {
    $(".nav-link-a").click(function () {
        // remove active classes from all
        $(".nav-link-a").removeClass("nav-link-a-active");
        // add active class to the menu item clicked
        $(this).addClass("nav-link-a-active");
        // remove the main-wrapper.toggle and hamburger.changed classes to set menu back to default
        $("#main-wrapper").removeClass("toggled"); // toggle main-wrapper
        $('.header-brand').removeClass("toggled"); // toggle header-container div
        $("#hamburger-toggle").removeClass('changed'); // toggle hamburger/x icon
    });
});
