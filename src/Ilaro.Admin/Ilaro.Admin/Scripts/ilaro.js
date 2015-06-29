﻿$(function () {
	$('.date-time-picker').datetimepicker();

	$('.date-picker').datetimepicker({
		pickTime: false
	});

	$('.time-picker').datetimepicker({
		pickDate: false
	});

	$('.numeric').spinedit({ value: 0 });

	$('select:not(.dual-list)').chosen({ disable_search_threshold: 10 });

	$('textarea.markdown').markdown();

	$('textarea.html').summernote({ height: 100 });

	$('.dual-list').bootstrapDualListbox({
		moveOnSelect: false
	});

	$('input[type=file]').bootstrapFileInput();

	$('th > span').tooltip();
	$('.open-modal').click(function (e) {
		e.preventDefault();
		$('#modal-image').attr('src', $(this).attr('href'));
		$('#modal').modal();
		return false;
	});
	$('.autoPostBack').change(function () {
		$(this).parents('form').submit();
	});
	$('legend .btn').click(function () {
		var $this = $(this);
		var $i = $this.children('i');
		if ($i.hasClass('glyphicon glyphicon-plus')) {
			$i.removeClass('glyphicon glyphicon-plus');
			$i.addClass('glyphicon glyphicon-minus');
			$this.parent().siblings('.fields').slideDown();
		}
		else {
			$i.removeClass('glyphicon glyphicon-minus');
			$i.addClass('glyphicon glyphicon-plus');
			$this.parent().siblings('.fields').slideUp();
		}
	});

	$('.create-foreign').click(function (e) {
		//e.preventDefault();

		//var url = $(this).attr('href');

		//$.get(url, function (data) {
		//	$('#foreign-modal .modal-body').html(data);
		//	$('#foreign-modal').modal('show');
		//});
	});
});