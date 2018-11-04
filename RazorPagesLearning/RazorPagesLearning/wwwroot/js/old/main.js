$(function () {
  new Lib();
  new Common();
  new Nav();
  new Search();
  new Calendar();
});

//////////////////*Library（jqueryプラグイン）////////////////// --/js/lib/~
// @spectrum.js --カラーピッカー --/js/lib/spectrum.js
// @jquery.cookie.js --クッキー操作 --/js/lib/jquery.cookie.js

var Lib = function () {
  //this.spectrum();    // 2018/08/22 M.Hoshino del this.spectrum is not a function
  this.datepicker();
};
Lib.prototype = {
  // 2018/08/20 M.Hoshino MessageAdminに移動しました。
  //spectrum: function () {
  //  //カラーピッカー
  //  $(function ($) {
  //    $("#picker_login").spectrum();
  //    $("#picker_home").spectrum();
  //  });
  //},
  //datepicker
  datepicker: function () {
    $('#datepicker').datepicker();
  }
};

//////////////////*Common//////////////////
var Common = function () {
  this.table.off();
};
Common.prototype = {
  table: {
    //テーブル共通
    off: function () {
      //.offになっている要素が存在する場合、チェックボックスをクリックすると.offが外れる
      if ($('table').hasClass('.off')) {
        console.log("test");
        $('table input[type="checkbox"]').change(function () {
          if ($(this).prop('checked')) {
            $(this).parents('tr').removeClass("off");
          } else {
            $(this).parents('tr').addClass("off");
          }
        });
      }
    }
  }
};
//////////////////*Layout//////////////////
//nav
var Nav = function () {
  this.accordion.state();
  this.accordion.animation();
  this.scale();
  $('body').css('visibility', 'visible');
};
Nav.prototype = {
  accordion: {
    animation: function () {
      //アコーディオンアニメーション
      $("#Nav .title").click(function () {
        $(this).next().slideToggle();
        $(this).find('.j-nav-icon').toggleClass('u-180deg');
      });
    },
    state: function () {
      //クッキー削除__デバッグ用
      // $.removeCookie('nav_currentPage');

      //初期値
      var currentPage = "";

      $('#Nav a').click(function () {
        getPageName = $(this).data('name');
        currentPage = getPageName;
        $.cookie.json = true;
        var setJson = {
          currentPage: currentPage
        };
        $.cookie('nav_currentPage', setJson);
      });

      $('#Header [href="account.html"]').click(function () {
        $.removeCookie('nav_currentPage');
      });

      //クッキーの自動JSON化を有効
      $.cookie.json = true;

      //クッキーに値がセットされている場合、初期値をクッキーと同じ値に変更
      if ($.cookie('nav_currentPage')) {
        var setPage = $.cookie('nav_currentPage').currentPage;
        $('#Nav').find('[data-name="' + setPage + '"]').css('background-color', 'rgb(52, 132, 173)');
      }
    }
  },
  scale: function () {
    //クッキー削除（デバッグ用）
    // $.removeCookie('nav_scale');

    //クッキーの値（メインエリアのマージン、フォントサイズ、グローバルナビの幅）を引き出し、Viewに反映
    (function initView() {
      if ($.cookie('nav_scale')) {
        var getCookie = $.cookie('nav_scale');
        $('#Main').css('margin-left', getCookie.scaleState.mainMargin);
        $("#Nav").css('font-size', getCookie.scaleState.navFontSize);
        $("#Nav").css('width', getCookie.scaleState.navWidth);
      }
    })();　

    //幅初期値
    var recetHeight = $('#Nav').height();
    var recetWidth = $("#Nav").width();

    //幅を記録する変数
    var recordWidth = $("#Nav").width();

    //@jquery-ui ドラッグすると要素の幅を変更する　幅の初期値を設定 
    $("#Nav").resizable({
      minHeight: recetHeight,
      maxHeight: recetHeight,
      minWidth: 155,
      maxWidth: 180,
    });

    //ブラウザの画面をリサイズした時、縮小ボタンの範囲を再定義
    $(window).resize(function () {
      $("#Nav").resizable({
        minHeight: $('#Nav').height(),
        maxHeight: $('#Nav').height()
      });
    });

    //ナビの幅が変わった時の処理
    $("#Nav").resize(function () {

      //現在のナビの幅を取得
      var currentWidth = $("#Nav").width();

      //記録された前回の幅と現在の幅の差分を取得
      var getDiffNum = recordWidth - currentWidth;

      //グローバルナビに隣接する、メインエリアのマージンを再設定
      $('#Main').css('margin-left', '-=' + getDiffNum);

      //幅のサイズに合わせてフォントサイズを変更
      if (currentWidth <= 165) {
        $("#Nav").css('font-size', '10px');
      } else if (currentWidth > 165 && currentWidth <= 173) {
        $("#Nav").css('font-size', '11px');
      } else if (currentWidth > 173 && currentWidth <= recetWidth) {
        $("#Nav").css('font-size', '12px');
      }

      //現在の幅を記録
      recordWidth = currentWidth;

      //クッキーの自動JSON化を有効
      $.cookie.json = true;

      //クッキーにナビの幅、メインエリアのマージン、フォントサイズを登録
      var setJson = {
        scaleState: {
          mainMargin: $('#Main').css('margin-left'),
          navWidth: $('#Nav').css('width'),
          navFontSize: $('#Nav').css('font-size')
        }
      };
      $.cookie('nav_scale', setJson);
    });
  }
};

//////////////////*Pages//////////////////
//検索
var Search = function () {
  this.addList();
  this.slide();
};
Search.prototype = {
  addList: function () {
    //検索項目一覧_プラスボタンスライド
    $('.j-serch-button__add').click(function () {
      $('.j-serch-button__add__area').slideToggle();
      $(this).toggleClass("isActive");
    });
  },
  slide: function () {
    //検索条件_スライドアニメーション
    var currentWidth = $('#Serch-list').width();
    $('.j-arrow__left').click(function () {
      $('#Serch-terms').animate({
        'margin-left': '-510px'
      });
      $('#Serch-list').animate({
        'width': '100%'
      });
      $('.j-arrow__right').animate({
        'left': '20px'
      });
      $('.j-slide__main').animate({
        'margin-left': '40px'
      });
    });
    $('.j-arrow__right').click(function () {
      $('#Serch-terms').animate({
        'margin-left': '0'
      });
      $('.j-arrow__right').animate({
        'left': '-610px',
      });
      $('#Serch-list').animate({
        'width': currentWidth + 'px'
      });
      $('.j-slide__main').animate({
        'margin-left': '0'
      });
    });
  }
};

//カレンダー
var Calendar = function () {
  this.changeColor();
};
Calendar.prototype = {
  //チェックボックス__休日のON/OFF
  changeColor: function () {
    $('.p-calendar').find("input[type='checkbox']:checked").parents('td').addClass("isActive");
    $('.p-calendar input[type="checkbox"]').change(function () {
      $('.p-calendar').find("input[type='checkbox']:checked").parents('td').addClass("isActive");
      $('.p-calendar').find("input:checkbox:not(:checked)").parents('td').removeClass("isActive");
    });
  }
};