function validateShippingInput() {
    if (document.getElementById("trackingNumber").value == "") {
        swal({
            icon: 'error',
            title: 'Oops..',
            text:'Please enter the tracking number'
        })
        return false;
    }
}

function EnterDataTracking() {
    var Tracking = document.getElementById("trackingNumber").value;
    if (Tracking == "") {
        Tracking.style.border = "1px solid green";
    } else {
        Tracking.style.border="thin solid #0000FF"
    }

    
}

function Delete(url) {
    swal({
        icon: 'warning',
        title: "Do You Want To Cancel The Order",
        text: "Click ok if you want to delete otherwise press cancel",
        buttons: true,
        dangermodel: true
    }).then((willRefund) => {
        if (willRefund) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.info(data.message);

                        setTimeout(reloadPage, 3000);
                    } else {
                        toastr.error(data.message)
                    }
                }
            })
        }
    });
}

function reloadPage() {
    location.reload();
}
