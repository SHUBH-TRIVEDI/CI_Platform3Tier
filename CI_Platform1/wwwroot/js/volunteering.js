let slideIndex = 1;
showSlides(slideIndex);

// Next/previous controls
function plusSlides(n) {
    showSlides(slideIndex += n);
}
function LoginAlert() {
    alert("login First")
}
// Thumbnail image controls
function currentSlide(n) {
    showSlides(slideIndex = n);
}

function showSlides(n) {
    let i;
    let slides = document.getElementsByClassName("mySlides");
    let dots = document.getElementsByClassName("demo");
    let captionText = document.getElementById("caption");
    if (n > slides.length) { slideIndex = 1 }
    if (n < 1) { slideIndex = slides.length }
    for (i = 0; i < slides.length; i++) {
        slides[i].style.display = "none";
    }
    for (i = 0; i < dots.length; i++) {
        dots[i].className = dots[i].className.replace(" active", "");
    }
    slides[slideIndex - 1].style.display = "block";
    dots[slideIndex - 1].className += " active";
    // captionText.innerHTML = dots[slideIndex-1].alt;
}



// Tabs js
const tabs = document.querySelectorAll('.tab');
const tabContents = document.querySelectorAll('.tab-content-item');

tabs.forEach(tab => {
    tab.addEventListener('click', function () {
        // remove active class from all tabs and tab content
        tabs.forEach(tab => tab.classList.remove('active'));
        tabContents.forEach(content => content.classList.remove('active'));

        // add active class to current tab and its corresponding tab content
        this.classList.add('active');
        document.getElementById(this.dataset.tabContent).classList.add('active');
    });
});


//--------------------------Add to Favroite Mission--------------------------------------------


function addToFav(missonid) {
    $.ajax({
        type: 'GET',
        url: "/Home/addToFavourite",
        data: { 'missonid': missonid },
        success: function (res) {
            $('.addFav').html($(res).find('.addFav').html());
         
        },
        error: function () {
            alert("please login first.");
        }
        
    });
}

function Apply(missonid) {
    $.ajax({
        type: 'GET',
        url: "/Home/Applied",
        data: { 'missonid': missonid },
        success: function (res) {
            $('.apply').html($(res).find('.apply').html());
            console.log("Mission Applied");
        },
        error: function () {
            alert("please login first.");
        }

    });
}

//Rating
function addRating(starId, missionId, Id) {


    $.ajax({
        url: '/Home/Addrating',
        type: 'POST',
        data: { missionId: missionId, Id: Id, rating: starId },
        success: function (response) {

            $('#starsr').html($(response).find('#starsr').html());
            $('#avgrating').html($(response).find('#avgrating').html());

        },
        error: function () {
            alert("could not add rating");
        }
    });
}

//Comments---------------------------
function PostComment() {
    var textBox = document.getElementById("commentarea");
    var commentVal = textBox.value;
    console.log(commentVal);
    const params = new URLSearchParams(window.location.search);
    const query = params.get('missionid');
    $.ajax({
        url: "/Home/PostComment",
        data: { missionId: query, Content: commentVal },
        success: function (result) {
            $('.commentdiv').html($(result).find('.commentdiv').html());
        },
        error: function () {
            alert("Error : Comment has not been posted");
        }
    });
}

//coworker
function sendmail(id) {
    const mail = Array.from(document.querySelectorAll('input[name="mail"]:checked')).map(el => el.id);
    var send = document.getElementById("sendmail");
    send.innerHTML = "Sending"
    $.ajax({
        url: '/Home/Sendmail',
        type: 'POST',
        data: { userid: mail, id: id },
        success: function (result) {
            alert("Recomendations sent successfully!");
            const checkboxes = document.querySelectorAll('input[name="mail"]:checked');
            checkboxes.forEach((checkbox) => {
                checkbox.checked = false;
            });
            send.innerHTML = "sent"
        },
        error: function () {
            // Handle error response from the server, e.g. show an error message to the user
            alert('Error: Could not recommend mission.');
        }
    });

}
