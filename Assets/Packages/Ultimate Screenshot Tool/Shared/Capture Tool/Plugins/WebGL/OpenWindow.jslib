var OpenWindowPlugin = {

		openWindow: function(uUrl) {
    	var url = Pointer_stringify(uUrl);
    	window.open(url, '_blank');
		},

		processImage: function(uImageDataUrl, uFileName, uType, display, download) {
      var imageDataUrl = Pointer_stringify(uImageDataUrl);
      var fileName = Pointer_stringify(uFileName);
			var type = Pointer_stringify(uType);

      function fixBinary (bin)
      {
        var length = bin.length;
        var buf = new ArrayBuffer(length);
        var arr = new Uint8Array(buf);
        for (var i = 0; i < length; i++)
              arr[i] = bin.charCodeAt(i);

        return buf;
      }

      	var contentType = 'image/' + type;
      	var binary = fixBinary(atob(imageDataUrl));
      	var data = new Blob([binary], {type: contentType});

      	var link = document.createElement('a');
      	link.download = fileName;
      	link.innerHTML = 'DownloadFile';
      	link.setAttribute('id', 'ImageDownloaderLink');

      	if(window.webkitURL != null)
        	link.href = window.webkitURL.createObjectURL(data);
      	else
      	{
        	link.href = window.URL.createObjectURL(data);
        	link.onclick = function()
        	{
          	var child = document.getElementById('ImageDownloaderLink');
            child.parentNode.removeChild(child);
          };
          link.style.display = 'none';
          document.body.appendChild(link);
      	}

				if(display)
					window.open(link.href, '_blank');

				if(download)
					link.click();
  	}
};

mergeInto(LibraryManager.library, OpenWindowPlugin);