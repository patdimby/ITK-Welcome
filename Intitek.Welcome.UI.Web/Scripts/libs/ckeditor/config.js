/**
 * @license Copyright (c) 2003-2019, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

CKEDITOR.editorConfig = function(config) {
    config.toolbarGroups = [
        { name: 'clipboard', groups: ['clipboard', 'undo'] },
        //{ name: 'editing',     groups: [ 'find', 'selection', 'spellchecker' ] },
        //{ name: 'links' },
        { name: 'insert' },
        //{ name: 'forms' },
        //{ name: 'tools' },
        //{ name: 'document', groups: ['mode', 'document', 'doctools'] },
        //{ name: 'others' },
        //'/',
        { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
        { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'] },
        { name: 'styles', groups: ['styles'] },
        { name: 'colors' },
        //{ name: 'about' }
    ];

    // config.removeButtons = 'About,NumberedList,Outdent,BidiLtr,Link,Templates,Form,Save,NewPage,Preview,Print,PasteText,PasteFromWord,Find,Replace,SelectAll,Checkbox,Radio,TextField,Textarea,Select,Button,ImageButton,HiddenField,CopyFormatting,RemoveFormat,BulletedList,Indent,CreateDiv,BidiRtl,Language,Unlink,Anchor,Image,Flash,Smiley,Iframe,ShowBlocks';
    config.removeButtons = 'PageBreak,About,BidiLtr,Link,Templates,Form,Save,NewPage,Preview,Print,PasteText,PasteFromWord,Find,Replace,SelectAll,Checkbox,Radio,TextField,Textarea,Select,Button,ImageButton,HiddenField,CopyFormatting,RemoveFormat,CreateDiv,BidiRtl,Language,Unlink,Anchor,Image,Flash,Smiley,Iframe,ShowBlocks,Styles,ckeditorfa';
    config.height = 90;
    config.removePlugins = 'elementspath';
    config.resize_enabled = false;
    config.extraPlugins = 'autogrow,ckeditorfa';
    config.allowedContent = true;
    config.contentsCss = ['/Scripts/libs/fonts/fontawesome/css/fontawesome-all.css', '/Scripts/libs/bootstrap/css/bootstrap.min.css', '/Scripts/libs/ckeditor/contents.css'];
    config.autoGrow_onStartup = true;
    config.autoGrow_minHeight = 90;
    // config.autoGrow_maxHeight = 600;
};