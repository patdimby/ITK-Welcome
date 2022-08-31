import { constants } from "./constants.js";

$(function() {
    $("#directeurAgence").autocomplete({
        appendTo: '.director',
        source: function(request, response) {
            $.ajax({
                url: constants.api_url + '?page=' + $("#directeurAgence").val(),
                type: "GET",
                dataType: "json",
                beforeSend: function() {
                    $("#directeurAgence").addClass('loading-input');
                },
                success: function(data) {
                    $("#directeurAgence").removeClass('loading-input');
                    response($.map(data.data, function(item) {
                        return { label: item.first_name + ' ' + item.last_name, value: item.email };
                    }));
                },
                error: function(d) {
                    $("#directeurAgence").removeClass('loading-input');
                }
            })
        },
        change: function(event, ui) {
            if (!ui.item) {
                this.value = ' ';
            }
            $(this).closest('form').bootstrapValidator('revalidateField', $(this).prop('name'));
        }
    });
    
    $("#directeurAgenceResponse").autocomplete({
        appendTo: '.directorAgency',
        source: function(request, response) {
            $.ajax({
                url: constants.api_url + '?page=' + $("#directeurAgenceResponse").val(),
                type: "GET",
                dataType: "json",
                beforeSend: function() {
                    $("#directeurAgenceResponse").addClass('loading-input');
                },
                success: function(data) {
                    $("#directeurAgenceResponse").removeClass('loading-input');
                    response($.map(data.data, function(item) {
                        return { label: item.first_name + ' ' + item.last_name, value: item.email };
                    }));
                },
                error: function(d) {
                    $("#directeurAgenceResponse").removeClass('loading-input');
                }
            })
        },
        change: function(event, ui) {
            if (!ui.item) {
                this.value = ' ';
            }
            $(this).closest('form').bootstrapValidator('revalidateField', $(this).prop('name'));
        }
    });    
    
    $("#directeurAgenceSubmission").autocomplete({
        appendTo: '.directorSubmission',
        source: function(request, response) {
            $.ajax({
                url: constants.api_url + '?page=' + $("#directeurAgenceSubmission").val(),
                type: "GET",
                dataType: "json",
                beforeSend: function() {
                    $("#directeurAgenceSubmission").addClass('loading-input');
                },
                success: function(data) {
                    $("#directeurAgenceSubmission").removeClass('loading-input');
                    response($.map(data.data, function(item) {
                        return { label: item.first_name + ' ' + item.last_name, value: item.email };
                    }));
                },
                error: function(d) {
                    $("#directeurAgenceSubmission").removeClass('loading-input');
                }
            })
        },
        change: function(event, ui) {
            if (!ui.item) {
                this.value = ' ';
            }
            $(this).closest('form').bootstrapValidator('revalidateField', $(this).prop('name'));
        }
    });        
});