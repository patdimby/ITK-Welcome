/*
 * Magazine sample
*/

function addPage(page, book) {
	var element = $('<div />', {});
	if (book.turn('addPage', element, page)) {
		element.html('<div class="gradient"></div><div class="loader"></div>');
		loadPage(page, element);
	}
}

function loadPage(page, pageElement) {
	var img = $('<img />');
	var options = $('.magazine').turn('options');
	img.mousedown(function (e) {
		e.preventDefault();
	});
	img.on('load', function () {
		$(this).css({ width: '100%', height: '100%' });
		$(this).appendTo(pageElement);
		pageElement.find('.loader').remove();
	});
	img.attr('src', options.images[page - 1 ]);
}

function isChrome() {
	return navigator.userAgent.indexOf('Chrome')!=-1;
}

function resizeViewport() {
	var width = $(window).width(),
		height = $(window).height(),
		options = $('.magazine').turn('options');

	$('.magazine').removeClass('animated');

	if ($('.magazine').turn('zoom')==1) {
		var bound = calculateBound({
			width: options.width,
			height: options.height,
			boundWidth: Math.min(options.width, width),
			boundHeight: Math.min(options.height, height)
		});

		if (bound.width%2!==0)
			bound.width-=1;
			
		if (bound.width!=$('.magazine').width() || bound.height!=$('.magazine').height()) {
			$('.magazine').turn('size', bound.width, bound.height);
			if ($('.magazine').turn('page')==1) $('.magazine').turn('peel', 'br');
		}

		$('.magazine').css({top: -bound.height/2 + 18, left: -bound.width/2});
	}

	$('.magazine').addClass('animated');
}

function numberOfViews(book) {
	return 1 + Math.ceil((book.turn('pages') - 1) / 2);
}

function getViewNumber(book, page) {
	return parseInt((page || book.turn('page'))/2 + 1, 10);
}

function calculateBound(d) {
	var bound = {width: d.width, height: d.height};
	if (bound.width>d.boundWidth || bound.height>d.boundHeight) {
		var rel = bound.width/bound.height;
		if (d.boundWidth/rel>d.boundHeight && d.boundHeight*rel<=d.boundWidth) {
			bound.width = Math.round(d.boundHeight*rel);
			bound.height = d.boundHeight;
		} else {
			bound.width = d.boundWidth;
			bound.height = Math.round(d.boundWidth/rel);
		}
	}
	return bound;
}