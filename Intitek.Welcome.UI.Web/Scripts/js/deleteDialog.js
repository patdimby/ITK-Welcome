$("#modalBtnDelete").on("click", function (e) {
    $.ajax({
        async: false,
        type: "POST",
        url: host + "Admin/" + $("#controller").val() + "/Delete",
        contentType: "application/json; charset=utf-8",
        data: '{"id": "' + $("#IDtoDelete").val() + '"}',
        dataType: "json",
        processData: true,
        success: function (result) {
            document.location = host + 'Admin/' + $("#controller").val() + '/Index';
        },

    });
});