"use strict";

import Translator from "../libs/translator/translator.js";
import { constants } from "../js/constants.js"

$(document).ready(function() {
    var lan = localStorage.getItem('lan');
    _translate(lan?lan:null);
});

$('#nav-lan .dropdown-item').click(function(evt) {
    openLoading();
    $("#nav-img").attr("src","../@Url.Content("~/Content/images/flags/" + this.id + ".png");
    localStorage.setItem('lan', this.id);
    $.each($("#nav-lan .fas"), function() {
        $(this).css({visibility:'hidden'});
    });
    $('#' + this.id + ' .fas').css({visibility:'visible'})
    _translate(this.id).then(
        closeLoading()
    );
});

export function _translate(lang) {
    if(lang === null) lang = 'fr'; 
    var translator = new Translator({
        persist: true,
        languages: ["fr", "en"],
        defaultLanguage: lang,
        detectLanguage: false,
        filesLocation: constants.url + "/assets/i18n"
    });
    localStorage.setItem('lan', lang);
    return new Promise(function(resolve) {
        $.each($("#nav-lan .fas"), function() {
            $(this).css({visibility:'hidden'});
        });
        $('#' + lang + ' .fas').css({visibility:'visible'})
        $("#nav-img").attr("src","../@Url.Content("~/Content/images/flags/" + lang + ".png");
        resolve(lang ? translator.load(lang) : translator.load());
    });
}
