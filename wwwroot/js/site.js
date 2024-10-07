
function onOverlayClick() {
    var dialog = document.getElementById("dialog").ej2_instances[0];
    var innerDialog = document.getElementById("innerDialog").ej2_instances[0];
    dialog.hide();
    innerDialog.hide();
};

function onAddEstimateButtonClick() {

    var dialog = document.getElementById("dialog").ej2_instances[0];
    dialog.header = "Add Estimate";
    dialog.content = "New client?";
    dialog.show();
};

function ondlgYesButtonClick() {
    window.location.href = "/Customer/Create";
};

function ondlgNoButtonClick() {
    window.location.href = "/Customer";
};

function onLoadFunctions() {

    $.ajax({
        url: '/DashboardPanel/GetOpenedWorkOrders',
        type: 'Post',
        dataType: 'json',
        success: function (workOrders) {
            if (workOrders != 0) {
                document.getElementById("activeWorkOrders").innerHTML = workOrders;
            }
        },
        error: function (ex) {
            console.log(ex);
        }
    });

    loadProfile();
}

function loadProfile() {


    var host = window.location.host;
    var url = 'https://' + host + '/Account/GetProfilePicturePath';

    $.ajax({
        url: url,
        type: 'Post',
        dataType: 'json',
        success: function (user) {
            console.log(user);


            if (user.profilePictureAltPath != null) {
                document.getElementById('userProfilePicture').src = user.profilePictureAltPath;
            }
            else {
                document.getElementById("userProfilePicture").src = "/images/blankProfilePicture.png";
            }

            document.getElementById("userFullName").innerHTML = user.fullName;
        },
        error: function (ex) {
            console.log(ex);
        }
    });
}

function getBackgroundProfilePicture() {

    $.ajax({
        url: '/Account/GetProfilePicturePath',
        type: 'Post',
        dataType: 'json',
        success: function (user) {
            console.log(user);


            if (user.profilePictureAltPath != null) {
                document.getElementById('profilePictureBackground').style.background = 'url(' + user.profilePictureAltPath + ')';
            }

        },
        error: function (ex) {
            console.log(ex);
        }
    });
}

function changeProfilePicture(inputId) {


    var input = document.getElementById(inputId).files[0];
    var formData = new FormData();
    formData.append('file', input);

    $.ajax({
        url: '/Account/ChangeProfilePic?handler=file',
        type: 'Post',
        data: formData,
        cache: false,
        processData: false,
        contentType: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (response) {

            window.location.reload(true);

        },
        error: function (ex) {
            console.log(ex);
        }
    });
}