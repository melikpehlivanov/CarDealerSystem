
(function ($) {
    "use strict";

    var $document = $(document),
        $window = $(window),
        $body = $('body'),
        $html = $('html'),
        windowWidth = window.innerWidth || $window.width(),
        $ttDesctopMenu = $('#tt-desctop-menu'),
        $ttFooterpMenu = $('#tt-footer-menu'),
        $ttMobilepMenu = $('#mobile-menu'),

        $ttPageContent = $('#tt-pageContent'),
        $ttFooter = $('tt-footer'),
        $ttHeader = $('#tt-header'),
        $ttStucknav = $('#tt-stuck'),
        $ttCompareSlide = $('#compareSlide'),

        $ttMobileQuickLinks = $ttHeader.find('.tt-mobile-quickLinks'),
        $ttMobileQuickLinksPopup = $ttHeader.find('.tt-quickLinks-popup'),

        ttAsideLocation = $('#tt-filters-aside'),
        ttFullwidthLocation = $('#tt-filters-fullwidth'),

    // Google map options
    googleMapOption = {
        latitude: 59.3,
        longitude: 18.0941403,
        zoom: 14,
        marker: [
          ['Best Hotel', 59.3, 18.0941403, 1, 'images/map-marker.png'],
        ]
    },

   // Template Blocks
    blocks = {
        ttHeaderDropdown: $ttHeader.find('.tt-dropdown-obj'),
        mainSlider: $('.mainSlider'),
        googleMapHeader: 'googleMapHeader',
        googleMapFooter: 'googleMapFooter',
        jsReviewsCarousel: $ttPageContent.find('.js-reviews-carousel'),
        ttPortfolioMasonry: $ttPageContent.find('.tt-portfolio-masonry'),
        mobileMenuToggle: $('.tt-menu-toggle'),
        sliderLayout: $ttPageContent.find('.slider-layout'),
        counterBlock: $ttPageContent.find('.counter-js'),
        ttVideoBlock: $('.tt-video-block'),
        ttFaq:$ttPageContent.find('.tt-faq'),
        ttAsideJs:$('#aside-js'),
        ttBtnToggleCol:$('#tt-btn-toggle-js'),
        ttListingFilters:$ttPageContent.find('.tt-filters-options'),
        ttCalendarDatepicker: $ttPageContent.find('.calendarDatepicker'),
        ttBlogMasonry: $ttPageContent.find('.tt-blog-masonry'),
        modalVideoProduct: $('#modalVideoProduct'),
        ttMobileProductSlider: $('.tt-mobile-product-slider'),
        ttTabs: $ttPageContent.find('.tt-tabs'),
        ttAsideGallery: $ttPageContent.find('.tt-aside-gallery'),
        ttProduct02: $ttPageContent.find('.tt-product-02'),
    };
    if (blocks.ttAsideGallery.length){
        blocks.ttAsideGallery.on('click', '.tt-img-thumbnails a', function(e){
           var $ttImgLarge = $(this).closest('.tt-aside-gallery').find('.tt-img-large img');

           if($(this).hasClass('tt-more')){
                $(this).hide().closest('li').siblings().removeClass('tt-more-hide');
                return false;
           };

           $ttImgLarge.hide().attr('src', $(this).attr('href'));
           $ttImgLarge.load(function(){
                $(this).fadeIn(300);
            });
            return false;
        });
   };
   // Compare open
    (function () {
        $(".compare-toggle").each(function () {
             $(this).on('click', function(e){
                var $objScroll =  $("#compareSlide").find('.slide-content'),
                    windowHeight = $(window).height();
                $("#compareSlide").toggleClass('open');
                $objScroll.height(windowHeight).perfectScrollbar();
                $body.addClass('modal-open');
                return false;
            });
        });
        $ttCompareSlide.find('.btn-close-slide').on('click', function(e){
            $ttHeader.find('.tt-dropdown-obj').removeClass('active');
            $('#compareSlide').removeClass('open');
            $body.removeClass('modal-open');
            return false;
        });
        $ttCompareSlide.find('.item-close').on('click', function(e){
            $(this).closest('.col-item').remove();
            return false;
        });
     }());

    // mobile short more
    (function(){
        $ttPageContent.find(".ws-short-btn").each(function(){
            $(this).on('click', function(e){
                $(this).prev('.ws-short-structure').addClass('is-open');
                $(this).remove();
                return false;
            });
        });
    }());

    // header - tt-dropdown-obj
     if (blocks.ttHeaderDropdown.length) {
        $ttHeader.on('click', '.tt-dropdown-toggle', function(e){
            $(this).closest('.tt-dropdown-obj').removeClass('active').siblings().removeClass('active');
            if($(e.target).hasClass('tt-dropdown-toggle')){
                $(this).closest('.tt-dropdown-obj').toggleClass('active');
                return false;
            };
            if($(e.target).hasClass('tt-close-item')){
                $(event.target).closest('li').remove();
            };
        });
        $ttHeader.on('click', '.tt-dropdown-menu .tt-btn-close', function(e){
            $(this).closest('.tt-dropdown-obj').removeClass('active');
            return false;
        });
    };

    //tabs
    $.fn.ttTabs = function (options) {
        function ttTabs(tabs) {
            var $tabs = $(tabs),
                $head = $tabs.find('.tt-tabs__head'),
                $head_ul = $head.find('> ul'),
                $head_li = $head_ul.find('> li'),
                $head_span = $head_li.find('> span'),
                $border = $head.find('.tt-tabs__border'),
                $body = $tabs.find('.tt-tabs__body'),
                $body_li = $body.find('> div'),
                anim_tab_duration = options.anim_tab_duration || 500,
                anim_scroll_duration = options.anim_scroll_duration || 500,
                breakpoint = 1025,
                scrollToOpenMobile = (options.scrollToOpenMobile !== undefined) ? options.scrollToOpenMobile : true,
                singleOpen = (options.singleOpen !== undefined) ? options.singleOpen : true,
                toggleOnDesktop = (options.toggleOnDesktop !== undefined) ? options.toggleOnDesktop : true,
                effect = (options.effect !== undefined) ? options.effect : 'slide',
                offsetTop = (options.offsetTop !== undefined) ? options.offsetTop : '',
                goToTab = options.goToTab,
                $btn_prev = $('<div>').addClass('tt-tabs__btn-prev disabled'),
                $btn_next = $('<div>').addClass('tt-tabs__btn-next'),
                btn_act = false;

            function _closeTab($li, desktop) {
                var anim_obj = {
                    duration: anim_tab_duration,
                    complete: function () {
                        $(this).removeAttr('style');
                    }
                };

                function _anim_func($animElem) {
                    if(effect === 'toggle') {
                        $animElem.hide().removeAttr('style');
                    } else if(effect === 'slide') {
                        $animElem.slideUp(anim_obj);
                    } else {
                        $animElem.slideUp(anim_obj);
                    }
                };

                var $animElem;

                if(desktop || singleOpen) {
                    $head_li.removeClass('active');
                    $animElem = $body_li.filter('.active').removeClass('active').find('> div').stop();

                    _anim_func($animElem);
                } else {
                    var index = $head_li.index($li);

                    $li.removeClass('active');
                    $animElem = $body_li.eq(index).removeClass('active').find('> div').stop();

                    _anim_func($animElem);
                }
            };

            function _openTab($li, desktop, beforeOpen, afterOpen, trigger) {
                var index = $head_li.index($li),
                    $body_li_act = $body_li.eq(index),
                    $animElem,
                    anim_obj = {
                        duration: anim_tab_duration,
                        complete: function () {
                            if(afterOpen) afterOpen($body_li_act);
                        }
                    };

                function _anim_func($animElem) {
                    if(beforeOpen) beforeOpen($li.find('> span'));

                    if(effect === 'toggle') {
                        $animElem.show();
                        if(afterOpen) afterOpen($body_li_act);
                    } else if(effect === 'slide') {
                        $animElem.slideDown(anim_obj);
                    } else {
                        $animElem.slideDown(anim_obj);
                    }
                };

                $li.addClass('active');
                $animElem = $body_li_act.addClass('active').find('> div').stop();

                _anim_func($animElem);
            };

            function _replaceBorder($this, animate) {
                if($this.length) {
                    var span_l = $this.get(0).getBoundingClientRect().left,
                        head_l = $head.get(0).getBoundingClientRect().left,
                        position = {
                            left: span_l - head_l,
                            width: $this.width()
                        };
                } else {
                    var position = {
                        left: 0,
                        width: 0
                    };
                }

                if(animate) $border.stop().animate(position, anim_tab_duration);
                else $border.stop().css(position);
            };

            function _correctBtns($li, func) {
                var span_act_l = $li.find('> span').get(0).getBoundingClientRect().left,
                    span_act_r = $li.find('> span').get(0).getBoundingClientRect().right,
                    head_pos = {
                        l: $head.get(0).getBoundingClientRect().left,
                        r: $head.get(0).getBoundingClientRect().right
                    };

                if(span_act_l < head_pos.l) {
                    _replace_slider(Math.ceil(head_pos.l - span_act_l), head_pos, false, function () {
                        func();
                    });
                } else if(span_act_r > head_pos.r) {
                    _replace_slider(Math.ceil(span_act_r - head_pos.r) * -1, head_pos, false, function () {
                        func();
                    });
                } else {
                    func();
                }
            };

            $head.on('click', '> ul > li > span', function (e, trigger) {
                var $this = $(this),
                    $li = $this.parent(),
                    wind_w = window.innerWidth,
                    desktop = wind_w > breakpoint,
                    trigger = (trigger === 'trigger') ? true : false;

                if($li.hasClass('active')) {
                    if(desktop && !toggleOnDesktop) return;

                    _closeTab($li, desktop);

                    _replaceBorder('', true);
                } else {
                    _correctBtns($li, function () {
                        _closeTab($li, desktop);

                        _openTab($li, desktop,
                            function($li_act) {
                                if(desktop) {
                                    var animate = !trigger;

                                    _replaceBorder($li_act, animate);
                                }
                            },
                            function ($body_li_act) {
                                if(!desktop && !trigger && scrollToOpenMobile) {
                                    var tob_t = $body_li_act.offset().top;
                                    $('html, body').stop().animate({ scrollTop: tob_t }, {
                                        duration: anim_scroll_duration
                                    });
                                }
                            }
                        );
                    });
                }
            });

            $body.on('click', '> div > span', function (e) {
                var $this = $(this),
                    $li = $this.parent(),
                    index = $body_li.index($li);

                $head_li.eq(index).find('> span').trigger('click');
            });

            function _toTab(tab, scrollTo, focus) {
                var wind_w = window.innerWidth,
                    desktop = wind_w > breakpoint,
                    header_h = 0,
                    $sticky = $(offsetTop),
                    $openTab = $head_li.filter('[data-tab="' + tab + '"]'),
                    $scrollTo = $(scrollTo),
                    toTab = {};

                if(desktop && $sticky.length) {
                    header_h = $sticky.height();
                }

                if(!$openTab.hasClass('active')) {
                    toTab = { scrollTop: $tabs.offset().top - header_h };
                }

                $('html, body').stop().animate(toTab, {
                    duration: anim_scroll_duration,
                    complete: function () {
                        _correctBtns($openTab, function () {
                            _closeTab($openTab, desktop);

                            _openTab($openTab, desktop,
                                function($li_act) {
                                    _replaceBorder($li_act, true);
                                },
                                function () {
                                    if ($scrollTo.length) {
                                        $('html, body').animate({ scrollTop: $scrollTo.offset().top - header_h }, {
                                            duration: anim_scroll_duration,
                                            complete: function () {
                                                var $focus = $(focus);

                                                if ($focus.length) $focus.focus();
                                            }
                                        });
                                    }
                                }
                            );
                        })
                    }
                });
            };

            if($.isArray(goToTab) && goToTab.length) {
                $(goToTab).each(function () {
                    var elem = this.elem,
                        tab = this.tab,
                        scrollTo = this.scrollTo,
                        focus = this.focus;

                    $(elem).on('click', function (e) {
                        _toTab(tab, scrollTo, focus);

                        e.preventDefault();
                        return false;
                    });
                });
            }

            function _btn_disabled(head_pos) {
                var span_pos = {
                    l: $head_li.first().find('> span').get(0).getBoundingClientRect().left,
                    r: $head_li.last().find('> span').get(0).getBoundingClientRect().right
                };

                if(span_pos.l < head_pos.l) $btn_prev.removeClass('disabled');
                else $btn_prev.addClass('disabled');

                if(span_pos.r > head_pos.r) $btn_next.removeClass('disabled');
                else $btn_next.addClass('disabled');
            };

            function _replace_slider(difference, head_pos, resize, afterReplace) {
                var ul_pos = parseInt($head_ul.css('left'), 10),
                    border_pos = parseInt($border.css('left'), 10),
                    duration = (!resize) ? anim_tab_duration : 0,
                    anim_pos = {
                        'left': ul_pos + difference
                    };

                if(resize) {
                    $head_ul.css(anim_pos);
                    _btn_disabled(head_pos);
                } else {
                    $border.animate({ 'left': border_pos + difference }, anim_tab_duration);

                    $head_ul.animate(anim_pos, {
                        duration: duration,
                        complete: function () {
                            _btn_disabled(head_pos);
                            if(afterReplace) afterReplace();
                            btn_act = false;
                        }
                    });
                }
            };

            $tabs.on('click', '.tt-tabs__btn-prev, .tt-tabs__btn-next', function () {
                var $btn = $(this);

                if($btn.hasClass('disabled') || btn_act) return;

                btn_act = true;

                var head_pos = {
                        l: $head.get(0).getBoundingClientRect().left,
                        r: $head.get(0).getBoundingClientRect().right
                    };

                if($btn.hasClass('tt-tabs__btn-next')) {
                    $head_span.each(function (i) {
                        var $this = $(this),
                            this_r = $this.get(0).getBoundingClientRect().right;

                        if(this_r > head_pos.r) {
                            _replace_slider(Math.ceil(this_r - head_pos.r) * -1, head_pos);
                            return false;
                        }
                    });
                } else if($btn.hasClass('tt-tabs__btn-prev')) {
                    $($head_span.get().reverse()).each(function (i) {
                        var $this = $(this),
                            this_l = $this.get(0).getBoundingClientRect().left;

                        if(this_l < head_pos.l) {
                            _replace_slider(Math.ceil(head_pos.l - this_l), head_pos);
                            return false;
                        }
                    });
                }
            });

            $(window).on('resize load', function () {
                var wind_w = window.innerWidth,
                    desktop = wind_w > breakpoint,
                    head_w = $head.innerWidth(),
                    li_w = 0;

                $head_li.each(function () {
                    li_w += $(this).innerWidth();
                });

                if(desktop) {
                    var $li_act = $head_li.filter('.active'),
                        $span_act = $li_act.find('> span');

                    if(!singleOpen && $span_act.length > 1) {
                        var $save_active = $li_act.first();

                        _closeTab('', desktop);
                        _openTab($save_active, desktop);
                    }

                    if(li_w > head_w) {
                        $head.addClass('slider').append($btn_prev).append($btn_next);

                        $head_ul.css({ 'margin-right' : (li_w - $head.innerWidth()) * -1 });

                        if($span_act.length) {

                            var span_act_r = $span_act.get(0).getBoundingClientRect().right,
                                span_last_r = $head_span.last().get(0).getBoundingClientRect().right,
                                head_pos = {
                                    l: $head.get(0).getBoundingClientRect().left,
                                    r: $head.get(0).getBoundingClientRect().right
                                };

                            if(span_act_r > head_pos.r) {
                                _replace_slider(Math.ceil(span_act_r - head_pos.r) * -1, head_pos, true);
                            } else if(span_last_r < head_pos.r) {
                                _replace_slider(head_pos.r - span_last_r, head_pos, true);
                            }

                            _replaceBorder($span_act, false);
                        }

                    } else {
                        $head_ul.removeAttr('style');
                        $btn_prev.remove();
                        $btn_next.remove();
                        $head.removeClass('slider');
                        _replaceBorder($span_act, false);
                    }

                    $head.css({ 'visibility': 'visible' });
                } else {
                    $border.removeAttr('style');
                }
            });

            $head_li.filter('[data-active="true"]').find('> span').trigger('click', ['trigger']);

            return $tabs;
        };

        var tabs = new ttTabs($(this).eq(0));

        return tabs;
    };
    if (blocks.ttTabs.length) {
          blocks.ttTabs.ttTabs({
            singleOpen: false,
            anim_tab_duration: 270,
            anim_scroll_duration: 500,
            toggleOnDesktop: false,
            scrollToOpenMobile: true,
            effect: 'slide',
            offsetTop: '.tt-header[data-sticky="true"]',
            goToTab: [
                {
                    elem: '.tt-product-head__review-count',
                    tab: 'review',
                    scrollTo: '.tt-review__comments'
                },
                {
                    elem: '.tt-product-head__review-add, .tt-review__head > a',
                    tab: 'review',
                    scrollTo: '.tt-review__form',
                    focus: '#reviewName'
                }
            ]
        });
    };

    if (blocks.ttMobileProductSlider.length) {
        blocks.ttMobileProductSlider.slick({
          dots: true,
          arrows: false,
          infinite: true,
          speed: 300,
          slidesToShow: 1,
          adaptiveHeight: true,
          lazyLoad: 'progressive',
        });
        if($html.hasClass('ie')){
          blocks.ttModalQuickView.each(function() {
              blocks.ttMobileProductSlider.slick("slickSetOption", "infinite", false);
          });
        };
    };

    //toggle col (listing-left-column.html)
    if (blocks.ttAsideJs.length && blocks.ttBtnToggleCol.length) {
        var $btnClose = blocks.ttAsideJs.find('.tt-btn-col-close');
        (function ttToggleCol() {
            blocks.ttBtnToggleCol.on('click', 'a', function (e) {
                var ttScrollValue = $body.scrollTop() || $html.scrollTop();
                blocks.ttAsideJs.toggleClass('column-open').perfectScrollbar();
                $body.css("top", - ttScrollValue).addClass("no-scroll").append('<div class="modal-filter"></div>');
                var modalFilter = $('.modal-filter').fadeTo('fast',1);
                if (modalFilter.length) {
                    modalFilter.on('click', function(){
                        $btnClose.trigger('click');
                    })
                }
                return false;
            });
            $btnClose.on('click', function(e) {
                e.preventDefault();
                blocks.ttAsideJs.removeClass('column-open').perfectScrollbar('destroy');
                var top = parseInt($body.css("top").replace("px", ""), 10) * -1;
                $body.removeAttr("style").removeClass("no-scroll").scrollTop(top);
                $html.removeAttr("style").scrollTop(top);
                $(".modal-filter").off().remove();
            });
            $window.on('resize', function(){
               if($body.hasClass('no-scroll')){
                    blocks.ttAsideJs.find(".tt-btn-col-close").trigger('click');
                };
            });
        })();
    };

    //popup on pages product single
    if (blocks.modalVideoProduct.length) {
         blocks.modalVideoProduct.on('show.bs.modal', function(e) {
            var relatedTarget = $(e.relatedTarget),
                attr = relatedTarget.attr('data-value'),
                attrPoster = relatedTarget.attr('data-poster'),
                attrType = relatedTarget.attr('data-type');

            if(attrType === "youtube" || attrType === "vimeo" || attrType === undefined){
              $('<iframe src="'+attr+'" allowfullscreen></iframe>').appendTo($(this).find('.modal-video-content'));
            };

            if(attrType === "video"){
              $('<div class="tt-video-block"><a href="#" class="link-video"></a><video class="movie" src="'+attr+'" poster="'+attrPoster+'" allowfullscreen></video></div>').appendTo($(this).find('.modal-video-content'));

            };
           ttVideoBlock();
        }).on('hidden.bs.modal', function () {
            $(this).find('.modal-video-content').empty();
        });
    };
    //video
    function ttVideoBlock() {
        $('.tt-video-block').on('click', function (e) {
            e.preventDefault();
            var myVideo = $(this).find('.movie')[0];
            if (myVideo.paused) {
              myVideo.play();
              $(this).addClass('play');
            } else {
              myVideo.pause();
              $(this).removeClass('play');
            }
        });
    };

    // Blog Masonr
    function gridGalleryMasonr() {
        // init Isotope
        var $grid = blocks.ttBlogMasonry.find('.tt-blog-init').isotope({
            itemSelector: '.element-item',
            layoutMode: 'masonry',
        });
        // layout Isotope after each image loads
        $grid.imagesLoaded().progress( function() {
          $grid.isotope('layout');
        });
        // filter functions
        var ttFilterNav =  blocks.ttBlogMasonry.find('.tt-filter-nav');
        if (ttFilterNav.length) {
            var filterFns = {
                ium: function() {
                  var name = $(this).find('.name').text();
                  return name.match(/ium$/);
                }
            };
            // bind filter button click
           ttFilterNav.on('click', '.button', function() {
                var filterValue = $(this).attr('data-filter');
                filterValue = filterFns[filterValue] || filterValue;
                $grid.isotope({
                  filter: filterValue
                });
                $(this).addClass('active').siblings().removeClass('active');
            });
        };
        var isotopShowmoreJs = $('.isotop_showmore_js .btn'),
            ttAddItem = $('.tt-add-item');
        if (isotopShowmoreJs.length && ttAddItem.length) {
            isotopShowmoreJs.on('click', function(e) {
                e.preventDefault();
                $.ajax({
                    url: 'ajax_post.php',
                    success: function(data) {
                      var $item = $(data);
                      ttAddItem.append($item);
                      $grid.isotope('appended', $item);
                      adjustOffset();
                    }
                });
                function adjustOffset(){
                    var offsetLastItem = ttAddItem.children().last().children().offset().top - 180;
                    $($body, $html).animate({
                        scrollTop: offsetLastItem
                    }, 500);
                };
                return false;
             });
        };
    };

    //calendarDatepicker(blog)
    if (blocks.ttCalendarDatepicker.length) {
        blocks.ttCalendarDatepicker.datepicker();
    };

    function moveFilterListing(windowWidth){
        windowWidth <= 1024 ? insertAsideLocation() : insertFullwidthLocation();

        function insertFullwidthLocation(){
            var objFullwidth = ttAsideLocation.children().detach();
            ttFullwidthLocation.append(objFullwidth);
        };
        function insertAsideLocation(){
            var objAside = ttFullwidthLocation.children().detach();
            ttAsideLocation.append(objAside);
        };
        if(!$('#slider-snap').hasClass('noUi-target')){
            initPriceSlider();
        };
    };

    //snapSlider
    function initPriceSlider(){
       var snapSlider = document.getElementById('slider-snap');
       if (snapSlider){
            noUiSlider.create(snapSlider, {
              start: [ 1000, 40000 ],
              snap: true,
              connect: true,
              range: {
                'min': 5000,
                '10%': 10000,
                '20%': 20000,
                '30%': 30000,
                '40%': 40000,
                '50%': 50000,
                'max': 50000
              }
            });
            var snapValues = [
              document.getElementById('slider-snap-value-lower'),
              document.getElementById('slider-snap-value-upper')
            ];
            snapSlider.noUiSlider.on('update', function( values, handle ) {
              snapValues[handle].innerHTML = values[handle];
            });
        };
    };

    if (blocks.ttListingFilters.length){
       blocks.ttListingFilters.on('click', '.tt-quantity a', function(e){
          $(this).toggleClass('active').siblings().removeClass('active');
          if($(e.target).hasClass('tt-grid-switch')){
              $("#tt-product-listing").addClass('tt-row-view').find('.tt-product, .tt-product-02').addClass('tt-view');
          } else {
            $("#tt-product-listing").removeClass('tt-row-view').find('.tt-product, .tt-product-02').removeClass('tt-view');
          };
          return false;
       });
    };

    if (blocks.ttFaq.length) {
        blocks.ttFaq.on('click', function(e){
          $(this).toggleClass('active');
          return false;
        });
    };
    //video(blog listing)
    if (blocks.ttVideoBlock.length) {
         $('.tt-video-block').on('click', function (e) {
            e.preventDefault();
            var myVideo = $(this).find('.movie')[0];
            if (myVideo.paused) {
              myVideo.play();
              $(this).addClass('play');
            } else {
              myVideo.pause();
              $(this).removeClass('play');
            }
        });
    };

    //product pages
    var elevateZoomWidget = {
      scroll_zoom: true,
      class_name: '.zoom-product',
      thumb_parent: $('#smallGallery'),
      scrollslider_parent: $('.slider-scroll-product'),
      checkNoZoom: function(){
        return $(this.class_name).parent().parent().hasClass('no-zoom');
      },
      init: function(type){
        var _ = this;
        var currentW = window.innerWidth || $(window).width();
        var zoom_image = $(_.class_name);
        var _thumbs = _.thumb_parent;
        _.initBigGalleryButtons();
        _.scrollSlider();

        if(zoom_image.length == 0) return false;
        if(!_.checkNoZoom()){
          var attr_scroll = zoom_image.parent().parent().attr('data-scrollzoom');
          attr_scroll = attr_scroll ? attr_scroll : _.scroll_zoom;
          _.scroll_zoom = attr_scroll == 'false' ? false : true;
          currentW > 575 && _.configureZoomImage();
          _.resize();
        }

        if(_thumbs.length == 0) return false;
        var thumb_type = _thumbs.parent().attr('class').indexOf('-vertical') > -1 ? 'vertical' : 'horizontal';
        _[thumb_type](_thumbs);
        _.setBigImage(_thumbs);
      },
      configureZoomImage: function(){
        var _ = this;
        $('.zoomContainer').remove();
        var zoom_image = $(this.class_name);
        zoom_image.each(function(){
          var _this = $(this);
          var clone = _this.removeData('elevateZoom').clone();
          _this.after(clone).remove();
        });
        setTimeout(function(){
          $(_.class_name).elevateZoom({
            gallery: _.thumb_parent.attr('id'),
            zoomType: "inner",
            scrollZoom: Boolean(_.scroll_zoom),
            cursor: "crosshair",
            zoomWindowFadeIn: 300,
            zoomWindowFadeOut: 300
          });
        }, 100);
      },
      resize: function(){
        var _ = this;
        $(window).resize(function(){
          var currentW = window.innerWidth || $(window).width();
          if(currentW <= 575) return false;
          _.configureZoomImage();
        });
      },
      horizontal: function(_parent){
        _parent.slick({
          infinite: true,
          dots: true,
          arrows: true,
          slidesToShow: 5,
          slidesToScroll: 1,
          responsive: [{
            breakpoint: 1200,
            settings: {
              slidesToShow: 4,
              slidesToScroll: 1
            }
          },
          {
            breakpoint: 992,
            settings: {
              slidesToShow: 4,
              slidesToScroll: 1
            }
          }]
        });
      },
      vertical: function(_parent){
        _parent.slick({
          vertical: true,
          infinite: true,
          slidesToShow: 5,
          slidesToScroll: 1,
          verticalSwiping: true,
          arrows: true,
          dots: false,
          centerPadding: "0px",
          customPaging: "0px",
          responsive: [{
            breakpoint: 1200,
            settings: {
              slidesToShow: 5,
              slidesToScroll: 1
            }
          },
          {
            breakpoint: 992,
            settings: {
              slidesToShow: 5,
              slidesToScroll: 1
            }
          },
          {
            breakpoint: 768,
            settings: {
              slidesToShow: 5,
              slidesToScroll: 1
            }
          }]
        });
      },
       initBigGalleryButtons: function(){
              var bigGallery = $('.bigGallery');
              if(bigGallery.length == 0) return false;
              $( 'body' ).on( 'mouseenter', '.zoomContainer',
                      function(){        bigGallery.find('button').addClass('show');        }
              ).on( 'mouseleave', '.zoomContainer',
                      function(){ bigGallery.find('button').removeClass('show'); }
              );
      },
      scrollSlider: function(){
        var _scrollslider_parent = this.scrollslider_parent;
        if(_scrollslider_parent.length == 0) return false;
        _scrollslider_parent.on('init', function(event, slick) {
          _scrollslider_parent.css({ 'opacity': 1 });
        });
        _scrollslider_parent.css({ 'opacity': 0 }).slick({
          infinite: false,
          vertical: true,
          verticalScrolling: true,
          dots: true,
          arrows: false,
          slidesToShow: 1,
          slidesToScroll: 1,
          responsive: [{
            breakpoint: 1200,
            settings: {
              slidesToShow: 1,
              slidesToScroll: 1
            }
          },
          {
            breakpoint: 992,
            settings: {
              slidesToShow: 1,
              slidesToScroll: 1
            }
          },
          {
            breakpoint: 768,
            settings: {
              slidesToShow: 1,
              slidesToScroll: 1
            }
          }]
        }).mousewheel(function(e) {
          e.preventDefault();
          e.deltaY < 0 ? $(this).slick('slickNext') : $(this).slick('slickPrev');
        });
      },
      setBigImage: function(_parent){
        var _ = this;
        _parent.find('a').on('click',function(e) {
          _.checkNoZoom() && e.preventDefault();
          var zoom_image = $(_.class_name);
          var getParam = _.checkNoZoom() ? 'data-image' : 'data-zoom-image';
          var setParam = _.checkNoZoom() ? 'src' : 'data-zoom-image';
          var big_image = $(this).attr(getParam);
          zoom_image.attr(setParam, big_image);

          if(!_.checkNoZoom()) return false;
          _parent.find('.zoomGalleryActive').removeClass('zoomGalleryActive');
          $(this).addClass('zoomGalleryActive');
        });
      }
    };
    elevateZoomWidget.init();

    //input type file
    $document.on('change', ':file', function() {
      var input = $(this),
          numFiles = input.get(0).files ? input.get(0).files.length : 1,
          label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
      input.trigger('fileselect', [numFiles, label]);
    });
    $(':file').on('fileselect', function(event, numFiles, label) {
        $(this).closest('.tt-input-file').addClass('tt-upload');
        var input = $(this).parents('.input-group').find(':text'),
            log = numFiles > 1 ? numFiles + ' files selected' : label;
        if( input.length ) {
            input.val(log);
        } else {
            if( log ) alert(log);
        }
    });

    // number counter
    if (blocks.counterBlock.length) {
         $window.scroll(function(){
            var ttCounterObj =  $('.tt-counter');

            ttCounterObj.each(function(){
              var cPos = $(this).offset().top,
                  topWindow = $window.scrollTop();

              if(cPos < topWindow + 800) {
                $('.tt-counter').countTo().removeClass('tt-counter');
              }
            });
        })
    };

    //mobile-quickLinks
    if($ttMobileQuickLinks.length && $ttMobileQuickLinksPopup.length){
        $('.tt-dropdown-menu').on('click', function(e){
            if($(e.target).hasClass('tt-btn-close')){
               $(event.target).closest('.col').removeClass('active').find(".btn-toggle").trigger('click');
            };
            if($(e.target).hasClass('tt-close-item')){
               $(event.target).closest('li').remove();
            };
        });
        $ttMobileQuickLinks.on('click', '.btn-toggle:not(.no-popup)', function(e){
          if($ttMobileQuickLinksPopup.children().length){
            $ttMobileQuickLinks.find('.btn-toggle.active').next('.quickLinks-layout').append($ttHeader.find('.tt-quickLinks-popup').children().detach());
          };
          if ($(this).hasClass('active')){
              $ttMobileQuickLinks.find('.btn-toggle').removeClass('active').closest('.col').removeClass('active');
              return false;
          };
          $ttMobileQuickLinks.find('.btn-toggle').removeClass('active').closest('.col').removeClass('active');
          $(this).addClass('active').closest('.col').addClass('active');
          $ttMobileQuickLinksPopup.append($(this).next('.quickLinks-layout').children().detach());
          return false;
        });

        $(document).mouseup(function(e){
            var div = $(".tt-cart.active");
            if (!div.is(e.target) && div.has(e.target).length === 0) {
                div.removeClass('active').find('.btn-toggle').removeClass('active');
            };
        });
    };

    //initStuck
    if ($ttStucknav.length && !$ttStucknav.hasClass('disabled')) {
        var $headerHolder = $ttHeader.find('.header-holder');
        $window.scroll(function(){
            var ttHeaderHeight = $ttHeader.innerHeight();
            if($window.scrollTop() > ttHeaderHeight){
                if ($ttStucknav.hasClass('stuck')) return false;
                $ttStucknav.addClass('stuck');
                $ttStucknav.find('.tt-stuck-row').append($headerHolder.find('.row').children().detach());
            } else {
                if (!$ttStucknav.hasClass('stuck')) return false;
                $ttStucknav.removeClass('stuck');
                $headerHolder.find('.row').append($ttStucknav.find('.tt-stuck-row').children().detach());
            };
        });
    };

    //desctope menu
    var ttDesctopMenu = $('#tt-desctop-menu');
    if(ttDesctopMenu.length){
        //is subMenu, is hover
        var ttMenuObjLevel_0 = ttDesctopMenu.find('ul > li');
        ttDesctopMenu.find('ul').parent('li').addClass('is-subMenu');
        ttMenuObjLevel_0.on( "mouseenter mouseleave", function(event){
           $(this).toggleClass("is-hover");
        });
        //is active
        var location = window.location.href,
            cur_url =  location.split('/').pop();

        ttDesctopMenu.find('li').each(function() {
            var link = $(this).find('a').attr('href');

            if (cur_url == link){
                $(this).addClass('is-active').closest('.is-subMenu').addClass('is-active');
            }
        });
    };
    function touchClickDesctope(){
        ttDesctopMenu.find('ul > li').each( function() {
            if($(this).hasClass('is-subMenu')){
                $(this).one("click", false);
            }
        });
    };

    // select custom
    var ttSelect = $('.tt-select');
    if (ttSelect.length) {
        ttSelect.each( function() {
          $(this).niceSelect();
        });
    };

    if (blocks.mainSlider.length) {
        mainSlider();
    };

    if(!ttFullwidthLocation.length){
        initPriceSlider();
    };

    $window.on('resize load', function () {
        var windowWidth = window.innerWidth || $window.width();
        alignmentArrows();
        if(ttAsideLocation.length && ttFullwidthLocation.length){
            moveFilterListing(windowWidth);
        }
        initPortfolioPopup();
    });
    $window.on('load', function () {
        var windowWidth = window.innerWidth || $window.width();

        if ($body.length) {
            $body.addClass('loaded');
        };

        if (blocks.ttBlogMasonry.length) {
          gridGalleryMasonr();
        };

        if (blocks.ttPortfolioMasonry.length) {
          gridPortfolioMasonr();
        };
    });

    function initPortfolioPopup() {
        var $obj = $ttPageContent.find('.tt-product-02');
        if ($obj.length){
            window.innerWidth <= 1024 ?  objMobile($obj) : objDesctop($obj);
        };
        function objMobile($obj){
           $obj.find('.tt-wrapper-description .tt-btn-zomm').magnificPopup({
             items: [
              {
                src: 'images/product_02/product_02_01.jpg'
              },
              {
                src: 'images/product_02/product_02_02.jpg'
              },
              {
                src: 'images/product_02/product_02_03.jpg'
              },
              {
                src: 'images/product_02/product_02_04.jpg'
              },
              {
                src: 'images/product_02/product_02_05.jpg'
              },
              {
                src: 'images/product_02/product_02_06.jpg'
              },
              {
                src: 'images/product_02/product_02_07.jpg'
              },
              {
                src: 'images/product_02/product_02_08.jpg'
              },
              {
                src: 'images/product_02/product_02_09.jpg'
              },
              {
                src: 'images/product_02/product_02_10.jpg'
              }
            ],
            gallery: {
              enabled: true
            },
            type: 'image'
          });
        };
        function objDesctop($obj){
             $obj.find('.tt-image-box .tt-btn-zomm').magnificPopup({
              items: [
              {
                src: 'images/product_02/product_02_01.jpg'
              },
              {
                src: 'images/product_02/product_02_02.jpg'
              },
              {
                src: 'images/product_02/product_02_03.jpg'
              },
              {
                src: 'images/product_02/product_02_04.jpg'
              },
              {
                src: 'images/product_02/product_02_05.jpg'
              },
              {
                src: 'images/product_02/product_02_06.jpg'
              },
              {
                src: 'images/product_02/product_02_07.jpg'
              },
              {
                src: 'images/product_02/product_02_08.jpg'
              },
              {
                src: 'images/product_02/product_02_09.jpg'
              },
              {
                src: 'images/product_02/product_02_10.jpg'
              }
            ],
            gallery: {
              enabled: true
            },
            type: 'image'
          });
        };
    };

    function alignmentArrows(){
        setTimeout(function() {
            $ttPageContent.find('.slick-alignment-arrows').each(function (){
                var objArrow =  $(this).find(".slick-prev, .slick-next");
                if(objArrow != undefined){
                    var workspaceHeight = parseInt($(this).find('.slick-track').innerHeight(), 10),
                        objArrowHeight =parseInt( objArrow.innerHeight(), 10),
                        correctData =  parseInt((workspaceHeight - objArrowHeight) / 2, 10);

                    objArrow.css('marginTop', correctData);
                };
            });
        }, 300);
    };
    // main slider
    function mainSlider() {
        var $el = blocks.mainSlider;
        $el.find('.slide').first().imagesLoaded({
          background: true
        }, function(){
          setTimeout(function () {
                $el.parent().find('.loading-content').addClass('disable');
          }, 1200);
        });
        $el.on('init', function (e, slick) {
          var $firstAnimatingElements = $('div.slide:first-child').find('[data-animation]');
          doAnimations($firstAnimatingElements);
        });
        $el.on('beforeChange', function (e, slick, currentSlide, nextSlide) {
          var $currentSlide = $('div.slide[data-slick-index="' + nextSlide + '"]');
          var $animatingElements = $currentSlide.find('[data-animation]');
          doAnimations($animatingElements);
        });
        $el.slick({
            arrows: false,
            dots: false,
            autoplay: true,
            autoplaySpeed: 5500,
            fade: true,
            speed: 1000,
            pauseOnHover: false,
            pauseOnDotsHover: true,
            responsive: [{
                breakpoint: 768,
                settings: {
                    arrows: false
                }
            },{
                breakpoint: 1025,
                settings: {
                  dots: false,
                  arrows: false
                }
            }]
        });
    };
    function doAnimations(elements) {
        var animationEndEvents = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
        elements.each(function () {
            var $this = $(this);
            var $animationDelay = $this.data('animation-delay');
            var $animationType = 'animated ' + $this.data('animation');
            $this.css({
              'animation-delay': $animationDelay,
              '-webkit-animation-delay': $animationDelay
            });
            $this.addClass($animationType).one(animationEndEvents, function () {
              $this.removeClass($animationType);
            });
            if ($this.hasClass('animate')) {
              $this.removeClass('animation');
            }
        });
    };
    $('.tt-slick-slider').each(function(){
         $(this).slick({
            arrows: true,
            dots: true,
             responsive: [
               {
                breakpoint: 1370,
                settings: {
                  arrows: false,
                  dots: true,
                }
              }]
         });
    });
    // carusel Review
    if (blocks.jsReviewsCarousel.length){
        blocks.jsReviewsCarousel.each(function(){
            var slick = $(this),
                itemQuantity = $(this).data('item');

            slick.slick({
              mobileFirst: false,
              slidesToShow: itemQuantity || 1,
              slidesToScroll: 1,
              infinite: true,
              arrows: true,
              dots: true,
              autoplay: true,
              autoplaySpeed: 5000,
              speed: 500,
              pauseOnHover: false,
              responsive: [
                   {
                    breakpoint: 1370,
                    settings: {
                      arrows: false,
                      dots: true,
                    }

                  },
                  {
                    breakpoint: 1025,
                    settings: {
                      slidesToShow: 1,
                       arrows: false,
                    }

                  }]
            });
        });
    };
    var boxReviewsImg = $('.box-reviews-img');
    if (boxReviewsImg.length && blocks.jsReviewsCarousel.length){
        $('.js-reviews-carousel').on('beforeChange', function(event, slick, currentSlide){
            boxReviewsImg.find('div').each(function(){
                if ($(this).hasClass('tt-show')){
                    $(this).removeClass('tt-show');
                };
            });
        });
        $('.js-reviews-carousel').on('afterChange', function(event, slick, currentSlide){
            boxReviewsImg.find('div').removeClass('tt-show');
            boxReviewsImg.find('div').each(function(){
                if ($(this).hasClass('tt-show')){
                    $(this).removeClass('tt-show');
                };
            });
            if (currentSlide == 0) {
                boxReviewsImg.addClass('tt-show').find(".slide-img01").addClass('tt-show');
            }
            if (currentSlide == 1) {
                boxReviewsImg.addClass('tt-show').find(".slide-img02").addClass('tt-show');
            }
            if (currentSlide == 2) {
                boxReviewsImg.addClass('tt-show').find(".slide-img03").addClass('tt-show');
            }
        });
         //button
        var ttSlickButton = $('.tt-slick-button');
        if (ttSlickButton.length) {
            ttSlickButton.find('.slick-next').on('click',function(e) {
                $('.js-reviews-carousel').slick('slickNext');
            });
            ttSlickButton.find('.slick-prev').on('click',function(e) {
                $('.js-reviews-carousel').slick('slickPrev');
            });
        };
    };
    // carusel
    var jsCarousel = $('.js-carousel');
    if (jsCarousel.length) {
        jsCarousel.each( function() {
            var slick = $(this),
                itemQuantity = $(this).data('item'),
                itemDots = $(this).data('dots');

            slick.slick({
                dots: itemDots || false,
                arrows: true,
                infinite: true,
                speed: 600,
                slidesToShow: itemQuantity || 4,
                slidesToScroll: itemQuantity || 4,
                adaptiveHeight: true,
                autoplay: true,
                autoplaySpeed: 5000,
                  responsive: [
                   {
                    breakpoint: 1370,
                    settings: {
                      slidesToShow: 4,
                      slidesToScroll: 4,
                      arrows: false,
                      dots: true,
                    }
                  },
                   {
                    breakpoint: 1270,
                    settings: {
                      slidesToShow: 4,
                      slidesToScroll: 4,
                      arrows: false,
                    }
                  },{
                    breakpoint: 1025,
                    settings: {
                      slidesToShow: 3,
                      slidesToScroll: 3,
                      arrows: false,
                    }
                  },
                  {
                    breakpoint: 791,
                    settings: {
                      slidesToShow: 2,
                      slidesToScroll: 2,
                      arrows: false,
                    }
                  }]
            });
        });
    };
    var jsCarouselBrand = $ttPageContent.find('.js-carousel-brand');
    if (jsCarouselBrand.length) {
         jsCarouselBrand.slick({
            dots:  false,
            arrows: true,
            infinite: true,
            speed: 500,
            slidesToShow:  8,
            slidesToScroll: 1,
            adaptiveHeight: true,
            autoplay: true,
            autoplaySpeed: 6000,
              responsive: [
               {
                breakpoint: 1370,
                settings: {
                  slidesToShow: 6,
                  slidesToScroll: 1,
                  arrows: false,
                  dots: true,
                }
              },
               {
                breakpoint: 1270,
                settings: {
                  slidesToShow: 5,
                  slidesToScroll: 1,
                  arrows: false,
                  dots: true,
                }
              },{
                breakpoint: 1025,
                settings: {
                  slidesToShow: 4,
                  slidesToScroll: 1,
                  arrows: false,
                  dots: true,
                }
              },
              {
                breakpoint: 576,
                settings: {
                  slidesToShow: 3,
                  slidesToScroll: 1,
                  arrows: false,
                  dots: true,
                }
              },
              {
                breakpoint: 420,
                settings: {
                  slidesToShow: 2,
                  slidesToScroll: 1,
                  arrows: false,
                  dots: true,
                }
              }]
        });
    };
    var jsCarouselNews = $ttPageContent.find('.js-carousel-news');
    if (jsCarouselNews.length) {
         jsCarouselNews.slick({
            dots:  true,
            arrows: true,
            infinite: true,
            speed: 500,
            slidesToShow:  3,
            slidesToScroll: 1,
            adaptiveHeight: true,
            autoplay: true,
            autoplaySpeed: 5000,
              responsive: [
               {
                    breakpoint: 1370,
                    settings: {
                      arrows: false,
                    }
              },
              {
                breakpoint: 1025,
                settings: {
                  slidesToShow: 2,
                  slidesToScroll: 1,
                  arrows: false,
                }
              },
              {
                breakpoint: 767,
                settings: {
                  slidesToShow: 1,
                  slidesToScroll: 1,
                  arrows: false,
                }
              }]
        });
    };
    var jsCarouselCol4 = $ttPageContent.find('.js-carousel-col-4');
     if (jsCarouselCol4.length) {
         jsCarouselCol4.slick({
            dots:  true,
            arrows: true,
            infinite: true,
            speed: 500,
            slidesToShow:  4,
            slidesToScroll: 1,
            adaptiveHeight: true,
            autoplay: true,
            autoplaySpeed: 6000,
              responsive: [
               {
                    breakpoint: 1370,
                    settings: {
                      arrows: false,
                    }
              },
              {
                breakpoint: 1229,
                settings: {
                  slidesToShow: 3,
                  slidesToScroll: 1,
                  arrows: false,
                }
              },
              {
                breakpoint: 1024,
                settings: {
                  slidesToShow: 2,
                  slidesToScroll: 1,
                  arrows: false,
                }
              },
              {
                breakpoint: 767,
                settings: {
                  slidesToShow: 2,
                  slidesToScroll: 1,
                  arrows: false,
                }
              },
               {
                breakpoint: 576,
                settings: {
                  slidesToShow: 1,
                  slidesToScroll: 1,
                  arrows: false,
                }
              }]
        });
    };
     var jsCarouselCol3 = $ttPageContent.find('.js-carousel-col-3');
     if (jsCarouselCol3.length) {
         jsCarouselCol3.slick({
            dots:  true,
            arrows: true,
            infinite: true,
            speed: 500,
            slidesToShow:  3,
            slidesToScroll: 1,
            adaptiveHeight: true,
            autoplay: true,
            autoplaySpeed: 6000,
              responsive: [
               {
                    breakpoint: 1370,
                    settings: {
                      arrows: false,
                    }
              },
              {
                breakpoint: 1229,
                settings: {
                  slidesToShow: 3,
                  slidesToScroll: 1,
                  arrows: false,
                }
              },
              {
                breakpoint: 1024,
                settings: {
                  slidesToShow: 2,
                  slidesToScroll: 1,
                  arrows: false,
                }
              },
              {
                breakpoint: 767,
                settings: {
                  slidesToShow: 1,
                  slidesToScroll: 1,
                  arrows: false,
                }
              }]
        });
    };

    if (blocks.sliderLayout.length) {
        var settings = {
            dots: true,
            arrows: false,
            infinite: true,
            speed: 500,
            slidesToShow: 1,
            slidesToScroll: 1,
        };
        if ($window.width() < 750) {
            blocks.sliderLayout.slick(settings);
        };
        $window.on('resize ready', function() {
            if ($window.width() > 750) {
              if (blocks.sliderLayout.hasClass('slick-initialized'))
                blocks.sliderLayout.slick('unslick');
                return
            }
            if (!blocks.sliderLayout.hasClass('slick-initialized'))
              return blocks.sliderLayout.slick(settings);
        });
    };

    // button back to top
    var ttBackToTop = $('#ttBackToTop');
    if (ttBackToTop.length) {
        ttBackToTop.each(function(){
            $(this).on('click',  function(e) {
                $('html, body').animate({
                  scrollTop: 0
                }, 500);
                return false;
            });
            $window.scroll(function() {
                $window.scrollTop() > 500 ? ttBackToTop.stop(true.false).addClass('tt-show') : ttBackToTop.stop(true.false).removeClass('tt-show');
            });
        });
    };

    // footer menu
    if ($ttDesctopMenu && $ttFooterpMenu){
        var ttDesktopMenu = $ttDesctopMenu.find('nav').clone();
        $ttFooterpMenu.append(ttDesktopMenu);
        //is active
        var location = window.location.href,
            cur_url =  location.split('/').pop();

        $ttFooterpMenu.find('li').each(function() {
            var link = $(this).find('a').attr('href');

            if (cur_url == link){
                $(this).addClass('is-active').addClass('is-active');
            }
        });
    };
    // mobile menu
    if ($ttDesctopMenu && blocks.mobileMenuToggle){
        var ttDesktopMenu = $ttDesctopMenu.find('ul').first().children().clone();

        $ttMobilepMenu.find('ul').append(ttDesktopMenu);
        blocks.mobileMenuToggle.initMM({
            enable_breakpoint: true,
            mobile_button: true,
            breakpoint: 1025
        });
    };

    // background image inline
    dataBg('[data-bg]');
    function dataBg(el) {
      $(el).each(function () {
        var $this = $(this),
          bg = $this.attr('data-bg');
        $this.css({
          'background-image': 'url(' + bg + ')'
        });
      });
    };

    // Detect Touch Devices
    window.mobileCheck = function () {
      var i = !1;
      return function (a) {
        (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4))) && (i = !0)
      }(navigator.userAgent || navigator.vendor || window.opera), i
    };
    var isTouch = 'ontouchstart' in window || navigator.msMaxTouchPoints;
    if (isTouch) {
      $('body').addClass('touch');
      touchClickDesctope();
    };

    //map
    $('.tt-map').each(function(){
        var btnToggle = $(this).find('.tt-btn-toggle');

        btnToggle.on('click',function(e){
          $(this).toggleClass('is-open').next('.tt-box-map').slideToggle(200);
          return false;
        });
    });

    //map
    if ($("#" + blocks.googleMapHeader).length) {
       createMapHeader(blocks.googleMapHeader, googleMapOption.zoom, googleMapOption.latitude, googleMapOption.longitude, googleMapOption.marker);
    };
    if ($("#" + blocks.googleMapFooter).length) {
       createMapFooter(blocks.googleMapFooter, googleMapOption.zoom, googleMapOption.latitude, googleMapOption.longitude, googleMapOption.marker);
    };
    // Google Map Start
    var mapStyle = [{
      featureType: "water",
      elementType: "geometry",
      stylers: [{
        color: "#ededed"
      }, {
        lightness: 17
      }]
    }, {
      featureType: "landscape",
      elementType: "geometry",
      stylers: [{
        color: "#f5f5f5"
      }, {
        lightness: 20
      }]
    }, {
      featureType: "road.highway",
      elementType: "geometry.fill",
      stylers: [{
        color: "#ffffff"
      }, {
        lightness: 17
      }]
    }, {
      featureType: "road.highway",
      elementType: "geometry.stroke",
      stylers: [{
        color: "#ffffff"
      }, {
        lightness: 29
      }, {
        weight: 0.2
      }]
    }, {
      featureType: "road.arterial",
      elementType: "geometry",
      stylers: [{
        color: "#ffffff"
      }, {
        lightness: 18
      }]
    }, {
      featureType: "road.local",
      elementType: "geometry",
      stylers: [{
        color: "#ffffff"
      }, {
        lightness: 16
      }]
    }, {
      featureType: "poi",
      elementType: "geometry",
      stylers: [{
        color: "#f5f5f5"
      }, {
        lightness: 21
      }]
    }, {
      featureType: "poi.park",
      elementType: "geometry",
      stylers: [{
        color: "#f7f7f7"
      }, {
        lightness: 21
      }]
    }, {
      elementType: "labels.text.stroke",
      stylers: [{
        visibility: "on"
      }, {
        color: "#ffffff"
      }, {
        lightness: 16
      }]
    }, {
      elementType: "labels.text.fill",
      stylers: [{
        saturation: 36
      }, {
        color: "#858585"
      }, {
        lightness: 40
      }]
    }, {
      elementType: "labels.icon",
      stylers: [{
        visibility: "off"
      }]
    }, {
      featureType: "transit",
      elementType: "geometry",
      stylers: [{
        color: "#f7f7f7"
      }, {
        lightness: 19
      }]
    }, {
      featureType: "administrative",
      elementType: "geometry.fill",
      stylers: [{
        color: "#f7f7f7"
      }, {
        lightness: 20
      }]
    }, {
      featureType: "administrative",
      elementType: "geometry.stroke",
      stylers: [{
        color: "#f7f7f7"
      }, {
        lightness: 17
      }, {
        weight: 1.2
      }]
    }];
    function createMapHeader(id, mapZoom, lat, lng, markers) {
      var mapOptions = {
        zoom: mapZoom,
        scrollwheel: false,
        center: new google.maps.LatLng(lat, lng),
        styles: mapStyle
      };
      var mapHeader = new google.maps.Map(document.getElementById(id), mapOptions);
      var count,
        locations = markers;
      for (count = 0; count < locations.length; count++) {
        new google.maps.Marker({
          position: new google.maps.LatLng(locations[count][1], locations[count][2]),
          map: mapHeader,
          title: locations[count][0],
          icon: locations[count][4]
        });
      }
    };
    function createMapFooter(id, mapZoom, lat, lng, markers) {
      var mapOptions = {
        zoom: mapZoom,
        scrollwheel: false,
        center: new google.maps.LatLng(lat, lng),
        styles: mapStyle
      };
      var mapHeader = new google.maps.Map(document.getElementById(id), mapOptions);
      var count,
        locations = markers;
      for (count = 0; count < locations.length; count++) {
        new google.maps.Marker({
          position: new google.maps.LatLng(locations[count][1], locations[count][2]),
          map: mapHeader,
          title: locations[count][0],
          icon: locations[count][4]
        });
      }
    };

   function gridPortfolioMasonr() {
        // init Isotope
        var $grid = blocks.ttPortfolioMasonry.find('.tt-portfolio-content').isotope({
            itemSelector: '.element-item',
            layoutMode: 'masonry',
        });
        // layout Isotope after each image loads
        $grid.imagesLoaded().progress( function() {
          $grid.isotope('layout').addClass('tt-show');
        });
        // filter functions
        var ttFilterNav =  blocks.ttPortfolioMasonry.find('.tt-filter-nav');
        if (ttFilterNav.length) {
            var filterFns = {
                ium: function() {
                  var name = $(this).find('.name').text();
                  return name.match(/ium$/);
                }
            };
            // bind filter button click
           ttFilterNav.on('click', '.button', function() {
                var filterValue = $(this).attr('data-filter');
                filterValue = filterFns[filterValue] || filterValue;
                $grid.isotope({
                  filter: filterValue
                });
                $(this).addClass('active').siblings().removeClass('active');
            });
        };
    };

})(jQuery);

$(function () {
    $('.tt-colorswatch-btn').on('click', function(e) {
        $(this).closest('.tt-colorswatch').toggleClass('tt-open-swatch');
        return false;
    });
    $('.js-swatch-color').on('click', function(e) {
        var color = $(this).attr('data-color');
        if(color != undefined){
            $('link[href*="css/style"]').attr('href','css/style-'+color+'.css');
        } else {
            $('link[href*="css/style"]').attr('href','css/style.css');
        };
        $('.js-swatch-color').removeClass('active');
        $(this).toggleClass('active');
        e.preventDefault();
    });
});