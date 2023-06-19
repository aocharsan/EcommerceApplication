
$(document).ready(function () {
    loadDataTable();

});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/admin/product/getall',
            type:'POST',
        },
        processing: true,
        serverSide: true,
        filter: true,
        "columns": [
            { data: 'title' ,"width":"25%"},
            { data: 'isbn', "width": "15%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'author', "width": "20%" },
            { data: 'category.name', "width": "15%" },
            {
                data: 'id',

                "render": function (data) {

                    return `<div class="w-75 btn-group" role="group">
                    
                      <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> EDIT </a>
                      <a onClick=Delete('/admin/product/delete/${data}') class="btn btn-danger mx-2"> <i class="bi bi-pencil-square"></i> DELETE </a>
                     
                           </div>`

                },
                 
                "width": "15%"
               
            }
    ]
    
});

    function Delete(url)
    {
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: url,
                    type:'DELETE',
                    success: function (data)
                    {  //message data from controller Json()

                        toastr.success(data.message);


                    }
                 })


            }
        })
    }






}



