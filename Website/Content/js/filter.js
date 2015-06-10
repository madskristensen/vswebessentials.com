function applyFilter(version) {

    var nbSectionHeader = $('.section-header').length;
    $(".section-header").each(function () {
        if ($(this).data("version") && $(this).data("version").indexOf(version) == -1) {
            $(this).hide();
            nbSectionHeader--;
        }
        else {
            $(this).show();
        }
    });

    var nbArticle = $('article').length;
    $("article").each(function () {
        if ($(this).data("version") && $(this).data("version").indexOf(version) == -1) {
            $(this).hide();
            nbArticle--;
        }
        else {
            $(this).show();
        }
    });

    if (nbArticle === 0 && nbSectionHeader === 0) {
        $(".section-header").each(function () {
            if (!$(this).data("version")) {
                $(this).hide();
            }
        });
        $('#noArticleAvailableWarning').show();
    }
    else {
        $(".section-header").each(function () {
            if (!$(this).data("version")) {
                $(this).show();
            }
        });
        $('#noArticleAvailableWarning').hide();
    }

    var fullVersionName;
    switch (version.trim()) {
        case "WE2015":
            fullVersionName = "Web Essentials 2015";
            break;
        case "WE2013":
            fullVersionName = "Web Essentials 2013";
            break;
        case "WE2012":
            fullVersionName = "Web Essentials 2012";
            break;
    }

    $("#filterMenuItem").html("Filter: " + fullVersionName + '&nbsp;<b class="caret"></b>');
}