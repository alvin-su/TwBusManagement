$(function () {

    //BEGIN JSTREEVIEW
   
   
    
    
   
    var to = false;
    
    
    
    //END JSTREEVIEW

    //BEGIN FAMILY TREEVIEW VERTICAL
    $('.family-tree-vertical li:first').show();
    $('.family-tree-vertical li').on('click', function (e) {
        var children = $(this).find('> ul > li');
        if (children.is(":visible")) children.hide('fast');
        else children.show('fast');
        e.stopPropagation();
    });
    //END FAMILY TREEVIEW VERTICAL

    //BEGIN FAMILY TREEVIEW HORIZONTAL
    $('.family-tree-horizontal li:first').show();
    $('.family-tree-horizontal li').on('click', function (e) {
        var children = $(this).find('> ul > li');
        if (children.is(":visible")) children.hide('fast');
        else children.show('fast');
        e.stopPropagation();
    });
    //END FAMILY TREEVIEW HORIZONTAL

    //BEGIN JQUERY TREETABLE
   
    $("#example-advanced").treetable({ expandable: true });
    // Highlight selected row
    $("#example-advanced tbody").on("mousedown", "tr", function() {
        $(".selected").not(this).removeClass("selected");
        $(this).toggleClass("selected");
    });
    // Drag & Drop Example Code
    //$("#example-advanced .file, #example-advanced .folder").draggable({
    //    helper: "clone",
    //    opacity: .75,
    //    refreshPositions: true, // Performance?
    //    revert: "invalid",
    //    revertDuration: 300,
    //    scroll: true
    //});
    $("#example-advanced .folder").each(function() {
        $(this).parents("#example-advanced tr").droppable({
            accept: ".file, .folder",
            drop: function(e, ui) {
                var droppedEl = ui.draggable.parents("tr");
                $("#example-advanced").treetable("move", droppedEl.data("ttId"), $(this).data("ttId"));
            },
            hoverClass: "accept",
            over: function(e, ui) {
                var droppedEl = ui.draggable.parents("tr");
                if(this != droppedEl[0] && !$(this).is(".expanded")) {
                    $("#example-advanced").treetable("expandNode", $(this).data("ttId"));
                }
            }
        });
    });
    $("form#reveal").submit(function() {
        var nodeId = $("#revealNodeId").val()

        try {
            $("#example-advanced").treetable("reveal", nodeId);
        }
        catch(error) {
            alert(error.message);
        }

        return false;
    });
    //END JQUERY TREETABLE

});