var viewModel = {
    hashCode: null,
    email: null,
    init: function () {
        $("#btn-login").click(function(e) {
            e.preventDefault();
            viewModel.loginUser();
            //viewModel.email = $("#email").val();
            //viewModel.hashPassword();    
        }); 
    },
    hashPassword: function () {
        var form = $('#__AjaxAntiForgeryForm');
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        $.ajax({
            type: "POST",
            url: "/Account/HashPasswordSha256",
            data: {
                password: $("#password").val(),
                __RequestVerificationToken: token
            },
            success: function (data) {
                viewModel.hashCode = data;
                viewModel.getUserData();
            },
            error: function(err) {
                console.log(err);
            }
        });
    },
    getUserData: function() {
        var url = "https://studservice.hogent.be" + "/" + viewModel.email + "/" + viewModel.hashCode;
        $.ajax({
            type: "GET",
            url: url,
            success: function (data) {
                console.log(data);
            },
            error: function (err) {
                console.log(err);
            }
        });
    },
    loginUser: function (data) {
        var form = $('#__AjaxAntiForgeryForm');
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        var loginViewModel = {
            Name: "student",
            Email: "student@student.hogent.be",
            Password: "P@ssword1"
        }
        $.ajax({
            type: "POST",
            url: "/Account/LoginOtherService",
            dataType: "json",
            contentType: 'application/json',
            data: JSON.stringify(loginViewModel),
            success: function (data) {
                console.log(data);
            },
            error: function (err) {
                console.log(err);
            }
        });
    }
}
$(document).ready(function() {
    viewModel.init();
});