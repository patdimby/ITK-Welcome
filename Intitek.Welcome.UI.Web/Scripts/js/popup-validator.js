validateMyForm();
function validateMyForm(){
  $('#formFilterName').bootstrapValidator({
    excluded: [':disabled'],
    fields: {
        valueInput: {
            validators: {
                notEmpty: {
                    message: '<span data-i18n="errorRequiredValue"></span>'
                },
                callback: {
                  callback: function(value, validator, $field) {
                      // Determine the numbers which are generated in captchaOperation
                      $field.removeClass('form-control-required');
                      console.log('here');
                      return true;
                  }
                }
              }
        }
    }
  }).on('success.form.bv', function(e) {
    $('body').click();
  });
}

$('#formFilterVersion').bootstrapValidator({
  excluded: [':disabled'],
  fields: {
      value: {
          validators: {
            notEmpty: {
                message: '<span data-i18n="errorRequiredValue"></span>'
            },
            callback: {
              callback: function(value, validator, $field) {
                  // Determine the numbers which are generated in captchaOperation
                  $field.removeClass('form-control-required');
                  return true;
              }
            }
          }
      }
  }
}).on('success.form.bv', function(e) {
  $('body').click();
});