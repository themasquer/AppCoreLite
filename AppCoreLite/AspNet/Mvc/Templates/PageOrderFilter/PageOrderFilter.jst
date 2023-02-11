$(function () {
	$(".order").show();

	var caretDown = '<i class="bi bi-caret-down"></i>';
	var caretUp = '<i class="bi bi-caret-up"></i>';
	$(".orderexpressionlink").each(function () {
		var orderExpressionLinkHtml = $(this).html();
		var orderExpression = $(".orderexpression option:selected").text();
		var isAscending = $(".isorderdirectionascending").prop("checked");
		if (orderExpressionLinkHtml.trim() == orderExpression) {
			if (isAscending) {
				$(this).html(orderExpressionLinkHtml + caretUp);
			} else {
				$(this).html(orderExpressionLinkHtml + caretDown);
			}
		}
	});

	$(".pagenumber").change(function () {
		$("#form").submit();
	});
	$(".recordsperpagecount").change(function () {
		$(".pagenumber").val("1");
		$("#form").submit();
	});
	$(".orderexpression").change(function () {
		$("#form").submit();
	});
	$(".isorderdirectionascending").change(function () {
		$("#form").submit();
	});
	$(".filter").blur(function () {
		$(".pagenumber").val("1");
	});
	$(".clear").click(function (event) {
		event.preventDefault();
		$(".pagenumber").val("1");
		$(".filter").val("");
		$("#form").submit();
	});
	$(".orderexpressionlink").click(function (event) {
		event.preventDefault();
		var linkHtml = $(this).html().replace(caretUp, "").replace(caretDown, "");
		$(".orderexpression option").each(function () {
			if ($(this).text() == linkHtml.trim()) {
				$(this).prop("selected", "selected");
			}
		});
		var isAscending = $(".isorderdirectionascending").prop("checked");
		isAscending = !isAscending;
		$(".isorderdirectionascending").prop("checked", isAscending);
		$("#form").submit();
	});

	$(".input").blur(function () {
		$(".pagenumber").prop("disabled", true);
		$(".pagenumber").val("1");
	});
	$(".search").click(function (event) {
		event.preventDefault();
		$(".pagenumber").val("1");
		$("#form").submit();
	});
	$(".export").click(function (event) {
		event.preventDefault();
		var formAction = $("#form").prop("action");
		$("#form").prop("action", $(this).prop("href"));
		$("#form").submit();
		$("#form").prop("action", formAction);
	});
});