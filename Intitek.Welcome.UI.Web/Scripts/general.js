//disable analytics for video.js
window.HELP_IMPROVE_VIDEOJS = false;
var COOOKIES_NAMES = {
    UserDocumentHash: 'UserDocument_Grid_HASH',
    UserDocumentCollapseState: 'UserDocumentCollapseState',
    UserDocumentCollapseState_1: 'UserDocumentCollapseState_1',
    UserDocumentCollapseState_2: 'UserDocumentCollapseState_2',
    UserDocumentCollapseState_3: 'UserDocumentCollapseState_3',
    UserDocumentCollapseState_4: 'UserDocumentCollapseState_4',
    ScrollTop: 'Window_Scroll_Top'
};

function getCurrentURL() {
    var url = window.location.href;
    //console.log('getCurrentURL', url);
    return url;
}

function submitForm(element) {
    $(element).closest("form").submit();
}