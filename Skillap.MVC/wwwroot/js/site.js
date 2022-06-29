


$(() => {
    var connection = new signalR.HubConnectionBuilder().withUrl("/postChatHub").build();
    //var url = new URL(this.)
    //var id = url.searchParams.get("id");
    var params = new URLSearchParams(location.search);
    var data = { id: params.get('id') };
   
    connection.start();
    connection.on("LoadComments", function () {
        LoadProdData();
    })

    LoadProdData();

    function LoadProdData() {
        var div = '';

        $.ajax({
            url: '/ViewPost',
            method: 'GET',
            success: (result) => {
                $.each(result, (v) => {
                    div += `<div>
                                <p>${v.Content} --- ${v.CreatedTime}</p>
                            </div>`;
                })

                $("#comment").html(div);
            },
            error: () => {
                console.log()
            }
        });
    }
})