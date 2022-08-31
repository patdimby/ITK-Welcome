//
var typeAccount = 1;
const signUpButton = document.getElementById('signUp');
const signInButton = document.getElementById('signIn');
const container = document.getElementById('container');

_validateLogin();
_validateVerification();
//
$('input[type=radio][name=account]').change(function() {
    $("#domain").prop('disabled', this.value == 1?true:false);
    this.value == 1?$('#domainDiv').addClass('hide'):$('#domainDiv').removeClass('hide');
    this.value == 1?$('#divForgotPassword').removeClass('hide'):$('#divForgotPassword').addClass('hide');
    typeAccount = this.value;
    _validateLogin();
});

//$("#buttonVerify").on("click", function () {
//    $('#verificationForm').submit();
//});

signUpButton.addEventListener('click', () => {
	container.classList.add("right-panel-active");
});

signInButton.addEventListener('click', () => {
	container.classList.remove("right-panel-active");
});

function clickBtnLogin() {
    $("#ValidateLoginSummary").hide();
}
function _validateLogin() {
    var checked = $("#RememberMe").is(":checked");
    if (checked)
        $("#RememberMe").val("true");
    else 
        $("#RememberMe").val("false");
    $('#loginForm').bootstrapValidator({
        excluded: [':disabled'],
        fields: {
            Login: {
                validators: {
                    notEmpty: {
                        message: '<span>' + ERRORREQUIREDEMAIL +'</span>'
                    },
                    emailAddress: {
                        message: '<span>' + ERRORINVALIDEMAIL +'</span>'
                    }
                }
            },
            Password: {
                validators: {
                    notEmpty: {
                        message: '<span>'+ ERRORREQUIREDPASSWORD +'</span>'
                    }
                }
            }
        }
    })
    .on('success.form.bv', function (e) {
        $("#ValidateLoginSummary").hide();
        openLoading();
    });
}

function _validateVerification(){
    $('#verificationForm').bootstrapValidator({
        excluded: [':disabled'],
        fields: {
            emailAd: {
                validators: {
                    notEmpty: {
                        message: '<span>' + ERRORREQUIREDEMAIL +'</span>'
                    },
                    emailAddress: {
                        regexp: /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,
                        //regexp: constants["email-regex"],
                            message: '<span>' + ERRORINVALIDEMAIL + '</<span>'
                    }
                }
            },
            passwordAd: {
                validators: {
                    notEmpty: {
                        message: '<span>' + ERRORREQUIREDPASSWORD  + '</span>'
                    }
                }
            },
            confirmPasswordAd: {
                validators: {
                    notEmpty: {
                        message: '<span>' + ERRORREQUIREDPASSWORD +'</span>'
                    },
                    callback: {
                        message: '<span>' + ERRORRPASSWORDMATCH + '</<span>',
                        callback:function(value){
                            if(value.localeCompare($('#password')[0].value)==0){
                                return true;
                            } else {
                                return false;
                            }
                        }
                    }
                }
            }
        }
    })
     //        .on('success.form.bv', function (e) {
    //    if($('#emailAd')[0].disabled){
    //        _redirectTo('pages/home.html');
    //    }else{
    //        openLoading(true);
    //        setTimeout(() => {
    //            $('#emailAd').prop('disabled', true);
    //            $('#password').prop('disabled', false);
    //            $('#confirmPassword').prop('disabled', false);
    //            $('#buttonVerify').addClass('hide');
    //            $('#div-password').removeClass('hide');
    //            $('#buttonCreateAccount').removeClass('hide');
    //            $('#div-confirmPassword').removeClass('hide');
    //        }, 2000);

    //    }
    //});
}