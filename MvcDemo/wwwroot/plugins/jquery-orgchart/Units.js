/*
Main Web Api demo is in WebApiDemo project of this solution. 
UnitsController.cs, Units.html and Units.js files in this project are only for MVC demo purpose.  
*/

var jQueryOrgchart;

var apiUrl = "/api/Units";

var filter = {
    language: 2, // 1: Turkish, 2: English
    showDetailTexts: false, // false: show TreeNode texts, true: show TreeNodeDetail texts
    showAbbreviations: false, // true: show TreeNodeDetail abbreviations, false: don't show TreeNodeDetail abbreviations
    showOnlyActive: true, // true: show TreeNodes whose active is true, false: show all TreeNodes
}

$(function () {
    setJqueryOrgchartHeight();
    getJqueryOrgchart();
    $(window).resize(function () {
        setJqueryOrgchartHeight();
    });
    $("input[name=language]").change(function () {
        filter.language = 1;
        if ($("input[name=language]:checked").val() == "english")
            filter.language = 2;
        getJqueryOrgchart();
    });
    $("input[name=showunitsorpositions]").change(function () {
        filter.showDetailTexts = false;
        if ($("input[name=showunitsorpositions]:checked").val() == "positions")
            filter.showDetailTexts = true;
        getJqueryOrgchart();
    });
    $("#showabbreviations").change(function () {
        filter.showAbbreviations = $("#showabbreviations").prop("checked");
        getJqueryOrgchart();
    });
    $("#showonlyactive").change(function () {
        filter.showOnlyActive = $("#showonlyactive").prop("checked");
        getJqueryOrgchart();
    });
    $(".modalbuttontoprightclose").click(function () {
        $(".modal").hide(300);
    });
    $(".modalbuttonclose").click(function () {
        $(".modal").hide(300);
    });
    $("#position").change(function () {
        if ($("#position").val() == "0") {
            newPosition(true);
        }
        else {
            newPosition(false);
        }
        $("#positionid").val($("#position").val());
        $("#positionerror").hide();
        $(".message").text("");
    });
    $(".modalbuttonsave").click(function () {
        saveUnit();
    });
    $(".modalpositionedit").click(function () {
        editPosition();
        $(".message").text("");
    });
    $(".modalpositiondelete").click(function () {
        deletePosition();
        $(".message").text("");
    });
    $("#parentunit").change(function () {
        $("#parentid").val($("#parentunit").val());
    });
});

function setJqueryOrgchartHeight() {
    var windowHeight = $(window).height();
    var newHeight = windowHeight - 245;
    $('#orgChartContainer').css('height', (newHeight) + 'px');
};

function getJqueryOrgchart() {
    $.ajax({
        type: "post",
        url: apiUrl + "/GetUnits",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(filter),
        success: function (result) {
            jQueryOrgchart = $('#orgChart').orgChart({
                data: result,
                showControls: true,
                allowEdit: false,
                onAddNode: function (node) {
                    createUnit(node);
                },
                onDeleteNode: function (node) {
                    deleteUnit(node.data.id);
                },
                newNodeText: "&nbsp;"
            });
        },
        error: function (result) {
            console.log("An error occured during AJAX request: " + result.responseText);
        }
    });
}

async function getUnit(id) {
    await $.ajax({
        type: "get",
        url: apiUrl + "/GetUnit/" + id,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            $("#id").val(result.id);
            $("#parentid").val(result.parentId);
            $("#level").val(result.treeNodeDetail.level);
            $("#positionid").val(result.treeNodeDetailId);
            $("#nameenglish").val(result.nameEnglish);
            $("#nameturkish").val(result.nameTurkish);
            $("#textenglish").val(result.textEnglish);
            $("#textturkish").val(result.textTurkish);
            $("#abbreviationenglish").val(result.abbreviationEnglish);
            $("#abbreviationturkish").val(result.abbreviationTurkish);
            $("#active").prop("checked", result.isActive);
        },
        error: function (result) {
            console.log("An error occured during AJAX request: " + result.responseText);
        }
    });
}

async function getPositions() {
    var positionsCount = 0;
    await $.ajax({
        type: "get",
        url: apiUrl + "/GetPositions/" + $("#level").val(),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            $("#position").empty();
            var positionId;
            var positionText;
            positionsCount = result.length;
            for (var i = 0; i < result.length; i++) {
                positionId = result[i].id;
                positionText = "Level " + result[i].level + ": ";
                if (result[i].textEnglish.trim() != '' || result[i].textTurkish.trim() != '') {
                    if (result[i].textEnglish.trim() != '') {
                        positionText += result[i].textEnglish.trim();
                        if (result[i].textTurkish.trim() != '') {
                            positionText += ' (' + result[i].textTurkish.trim() + ')';
                        }
                    }
                    else {
                        positionText += result[i].textTurkish.trim();
                    }
                }
                $("#position").append('<option value="' + positionId + '">' + positionText + '</option >');
            }
            $("#position").append('<option value="0">-- New Position --</option >');
            positionsCount = result.length;
        },
        error: function (result) {
            console.log("An error occured during AJAX request: " + result.responseText);
        }
    });
    return positionsCount;
}

async function createUnit(node) {
    clearModalInputs();
    $(".message").text("");
    $("#nameerror").hide();
    $("#positionerror").hide();
    $("#id").val("0");
    $("#parentid").val(node.data.id);
    $("#level").val(node.data.level + 1);
    $("#operation").val("create");
    $(".modal-header").removeClass("modalheadereditcolor").addClass("modalheadercreatecolor");
    $(".modal-title").html("Create Unit");
    $(".modalbuttonclose").removeClass("btn-outline-primary").addClass("btn-outline-success");
    $(".modalbuttonsave").removeClass("btn-primary").addClass("btn-success");
    $(".modallabel").removeClass("modallabeledit").addClass("modallabelcreate");
    var positionsCount = await getPositions();
    $("#positionid").val($("#position").val());
    newPosition(positionsCount == 0 ? true : false);
    $(".parentunit").hide();
    $(".modal").show(300);
}

async function editUnit(id) {
    clearModalInputs();
    $(".message").text("");
    $("#nameerror").hide();
    $("#positionerror").hide();
    await getUnit(id);
    await getPositions();
    $("#position").val($("#positionid").val());
    $("#operation").val("edit");
    $(".modal-header").removeClass("modalheadercreatecolor").addClass("modalheadereditcolor");
    $(".modal-title").html("Edit Unit");
    $(".modalbuttonclose").removeClass("btn-outline-success").addClass("btn-outline-primary");
    $(".modalbuttonsave").removeClass("btn-success").addClass("btn-primary");
    $(".modallabel").removeClass("modallabelcreate").addClass("modallabeledit");
    newPosition(false);
    await getParentUnits();
    $("#parentunit").val($("#parentid").val());
    $(".modal").show(300);
}

function clearModalInputs() {
    $(".modalinput").each(function () {
        $(this).val("");
    });
    $(".modalinputcheckbox").prop("checked", true);
}

function clearModalPositionInputs() {
    $(".modalpositioninput").each(function () {
        $(this).val("");
    });
}

function newPosition(show) {
    clearModalPositionInputs();
    if (show) {
        $(".modalpositionedit").hide();
        $(".modalpositiondelete").hide();
        $(".position").show();
    } else {
        $(".modalpositionedit").show();
        $(".modalpositiondelete").show();
        $(".position").hide();
    }
}

async function saveUnit() {
    try {
        if (validateUnit()) {
            if ($("#operation").val() == "create") {
                await addUnit();
            } else {
                await updateUnit();
            }
        }
    }
    catch (error) {
        $(".message").text(error.responseText);
    }
}

function validateUnit() {
    var result = true;
    $("#nameerror").hide();
    $("#positionerror").hide();
    if ($("#nameenglish").val().trim() == "" && $("#nameturkish").val().trim() == "") {
        $("#nameerror").show();
        result = false;
    }
    if ($("#position").val() == "0" && ($("#positiontextenglish").val().trim() == "" && $("#positiontextturkish").val().trim() == "")) {
        $("#positionerror").show();
        result = false;
    }
    return result;
}

function setPosition() {
    var position = {
        id: $("#positionid").val(),
        textTurkish: $("#positiontextturkish").val(),
        textEnglish: $("#positiontextenglish").val(),
        level: $("#level").val()
    };
    return position;
}

function setUnit(position) {
    var unit = {
        id: $("#id").val(),
        parentId: $("#parentid").val(),
        nameTurkish: $("#nameturkish").val(),
        nameEnglish: $("#nameenglish").val(),
        textTurkish: $("#textturkish").val(),
        textEnglish: $("#textenglish").val(),
        abbreviationTurkish: $("#abbreviationturkish").val(),
        abbreviationEnglish: $("#abbreviationenglish").val(),
        treeNodeDetailId: position.id,
        treeNodeDetail: position,
        isActive: $("#active").prop("checked")
    }
    return unit;
}

async function addUnit() {
    var position = setPosition();
    var unit = setUnit(position);
    await $.ajax({
        type: "post",
        url: apiUrl + "/PostUnit",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(unit),
        success: function () {
            $(".modal").hide(300);
            getJqueryOrgchart();
        },
        error: function (result) {
            console.log("An error occured during AJAX request: " + result.responseText);
        }
    });
}

async function updateUnit() {
    var position = setPosition();
    var unit = setUnit(position);
    await $.ajax({
        type: "post",
        url: apiUrl + "/PutUnit/" + unit.id,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(unit),
        success: function () {
            $(".modal").hide(300);
            getJqueryOrgchart();
        },
        error: function (result) {
            console.log("An error occured during AJAX request: " + result.responseText);
        }
    });
}

function deleteUnit(id) {
    alertify.confirm("Warning!",
        "Are you sure you want to delete this unit?",
        function () { // OK
            $.ajax({
                type: "post",
                url: apiUrl + "/DeleteUnit/" + id,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function () {
                    getJqueryOrgchart();
                },
                error: function (result) {
                    console.log("An error occured during AJAX request: " + result.responseText);
                    alertify.alert("Error!", result.responseText, function () { // OK

                    });
                }
            });
        },
        function () { // Cancel

        }
    ).set('labels', { ok: 'Yes', cancel: 'No' });
}

function editPosition() {
    $.ajax({
        type: "get",
        url: apiUrl + "/GetPosition/" + $("#positionid").val(),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            $("#positiontextenglish").val(result.textEnglish);
            $("#positiontextturkish").val(result.textTurkish);
            $(".position").show();
        },
        error: function (result) {
            console.log("An error occured during AJAX request: " + result.responseText);
        }
    });
}

function deletePosition() {
    alertify.confirm("Warning!",
        "Are you sure you want to delete this position?",
        function () { // OK
            $.ajax({
                type: "post",
                url: apiUrl + "/DeletePosition/" + $("#positionid").val(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: async function (result) {
                    await getPositions($("#level").val());
                    $("#position").val("0");
                    $("#positionid").val("0");
                    newPosition(true);
                    $(".message").text(result);
                },
                error: function (result) {
                    $(".message").text(result.responseText);
                    console.log("An error occured during AJAX request: " + result.responseText);
                }
            });
        },
        function () { // Cancel

        }
    ).set('labels', { ok: 'Yes', cancel: 'No' });
}

async function getParentUnits() {
    $(".parentunit").hide();
    if ($("#parentid").val() != "0") {
        await $.ajax({
            type: "get",
            url: apiUrl + "/GetParentUnits/" + ($("#level").val() - 1),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                $("#parentunit").empty();
                var unitId;
                var unitName;
                for (var i = 0; i < result.length; i++) {
                    unitId = result[i].id;
                    unitName = "";
                    if (result[i].nameEnglish.trim() != '' || result[i].nameTurkish.trim() != '') {
                        if (result[i].nameEnglish.trim() != '') {
                            unitName += result[i].nameEnglish.trim();
                            if (result[i].textEnglish.trim() != '') {
                                unitName += ' ' + result[i].textEnglish.trim();
                            }
                            if (result[i].nameTurkish.trim() != '') {
                                if (result[i].textTurkish.trim() != '') {
                                    unitName += ' (' + result[i].nameTurkish.trim() + ' ' + result[i].textTurkish.trim() + ')';
                                } else {
                                    unitName += ' (' + result[i].nameTurkish.trim() + ')';
                                }
                            }
                        }
                        else {
                            if (result[i].textTurkish.trim() != '') {
                                unitName += result[i].nameTurkish.trim() + ' ' + result[i].textTurkish.trim();
                            } else {
                                unitName += result[i].nameTurkish.trim();
                            }
                        }
                    }
                    $("#parentunit").append('<option value="' + unitId + '">' + unitName + '</option >');
                }
                $(".parentunit").show();
            },
            error: function (result) {
                console.log("An error occured during AJAX request: " + result.responseText);
            }
        });
    }
}