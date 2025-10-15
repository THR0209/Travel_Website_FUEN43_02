/**
 * This configuration was generated using the CKEditor 5 Builder. You can modify it anytime using this link:
 * https://ckeditor.com/ckeditor-5/builder/?redirect=portal#installation/NoJgNARCB0Ds0BYKQIwhABgGwJQVhFkIGYNY9jYAOEATjxuIWNqtso1tqxDY1yzIIALwAWyDGGAowkyTJkYAuqiwpaAIwDGVCEqA
 */

/**
 * This configuration was generated using the CKEditor 5 Builder. You can modify it anytime using this link:
 * https://ckeditor.com/ckeditor-5/builder/?redirect=portal#installation/NoJgNARCB0Ds0BYKQIwhABgGwFZYJCwwA4icsQUVjYVZiBOBAklhjBgZgxK05GQQAXgAtkGMMBRgJE6dIwBdSAFMARgBNYsTgwiKgA==
 */

const {
	ClassicEditor,
	Alignment,
	Autoformat,
	AutoImage,
	AutoLink,
	Autosave,
	Base64UploadAdapter,
	BlockQuote,
	Bold,
	Bookmark,
	Code,
	CodeBlock,
	Emoji,
	Essentials,
	FindAndReplace,
	FontBackgroundColor,
	FontColor,
	FontFamily,
	FontSize,
	FullPage,
	Fullscreen,
	GeneralHtmlSupport,
	Heading,
	Highlight,
	HorizontalLine,
	HtmlComment,
	HtmlEmbed,
	ImageBlock,
	ImageCaption,
	ImageEditing,
	ImageInline,
	ImageInsert,
	ImageInsertViaUrl,
	ImageResize,
	ImageStyle,
	ImageTextAlternative,
	ImageToolbar,
	ImageUpload,
	ImageUtils,
	Indent,
	IndentBlock,
	Italic,
	Link,
	LinkImage,
	List,
	ListProperties,
	MediaEmbed,
	Mention,
	PageBreak,
	Paragraph,
	PasteFromOffice,
	PlainTableOutput,
	RemoveFormat,
	ShowBlocks,
	SourceEditing,
	SpecialCharacters,
	SpecialCharactersArrows,
	SpecialCharactersCurrency,
	SpecialCharactersEssentials,
	SpecialCharactersLatin,
	SpecialCharactersMathematical,
	SpecialCharactersText,
	Strikethrough,
	Subscript,
	Superscript,
	Table,
	TableCaption,
	TableCellProperties,
	TableColumnResize,
	TableLayout,
	TableProperties,
	TableToolbar,
	TextTransformation,
	TodoList,
	Underline
} = window.CKEDITOR;

const LICENSE_KEY =
	'eyJhbGciOiJFUzI1NiJ9.eyJleHAiOjE3ODc3MDIzOTksImp0aSI6IjdlZGRlM2U4LWVkYzEtNGQ2OS05Yjk4LWM3Y2Y5M2QwOGZiNSIsImxpY2Vuc2VkSG9zdHMiOlsiMTI3LjAuMC4xIiwibG9jYWxob3N0IiwiMTkyLjE2OC4qLioiLCIxMC4qLiouKiIsIjE3Mi4qLiouKiIsIioudGVzdCIsIioubG9jYWxob3N0IiwiKi5sb2NhbCJdLCJ1c2FnZUVuZHBvaW50IjoiaHR0cHM6Ly9wcm94eS1ldmVudC5ja2VkaXRvci5jb20iLCJkaXN0cmlidXRpb25DaGFubmVsIjpbImNsb3VkIiwiZHJ1cGFsIl0sImxpY2Vuc2VUeXBlIjoiZGV2ZWxvcG1lbnQiLCJmZWF0dXJlcyI6WyJEUlVQIiwiRTJQIiwiRTJXIl0sInZjIjoiNjYwMDY1MWMifQ.1gIWQnek6lVZXNjcrskK66Vy-hWaKTe-1_UL-ToV_Q6wvwQZHUR8SIivOqgFOJf-RskMDtfWxtl4mX1qnvYi0w';
      //'eyJhbGciOiJFUzI1NiJ9.eyJleHAiOjE3ODc3MDIzOTksImp0aSI6ImJlNjYxZGMzLWE2MjMtNDc2NS04NjY2LTNkMTI0OWEwY2QwZiIsInVzYWdlRW5kcG9pbnQiOiJodHRwczovL3Byb3h5LWV2ZW50LmNrZWRpdG9yLmNvbSIsImRpc3RyaWJ1dGlvbkNoYW5uZWwiOlsiY2xvdWQiLCJkcnVwYWwiXSwiZmVhdHVyZXMiOlsiRFJVUCIsIkUyUCIsIkUyVyJdLCJ2YyI6IjAxNzRhMzE0In0.1lL04Wyy - qcTHGt81tE18PgI0xGv - qvo2kNPfzS1BobdLe2V00BhzOg - fP17kD9ZYXyZBMyoGv88EhYSyaI6XA';
const editorConfig = {
	toolbar: {
		items: [
			'undo',
			'redo',
			'|',
			'sourceEditing',
			'showBlocks',
			'|',
			'heading',
			'|',
			'fontSize',
			'fontFamily',
			'fontColor',
			'fontBackgroundColor',
			'|',
			'bold',
			'italic',
			'underline',
			'|',
			'link',
			'insertImage',
			'insertTable',
			'insertTableLayout',
			'highlight',
			'blockQuote',
			'codeBlock',
			'|',
			'alignment',
			'|',
			'bulletedList',
			'numberedList',
			'todoList',
			'outdent',
			'indent'
		],
		shouldNotGroupWhenFull: false
	},
	plugins: [
		Alignment,
		Autoformat,
		AutoImage,
		AutoLink,
		Autosave,
		Base64UploadAdapter,
		BlockQuote,
		Bold,
		Bookmark,
		Code,
		CodeBlock,
		Emoji,
		Essentials,
		FindAndReplace,
		FontBackgroundColor,
		FontColor,
		FontFamily,
		FontSize,
		FullPage,
		Fullscreen,
		GeneralHtmlSupport,
		Heading,
		Highlight,
		HorizontalLine,
		HtmlComment,
		HtmlEmbed,
		ImageBlock,
		ImageCaption,
		ImageEditing,
		ImageInline,
		ImageInsert,
		ImageInsertViaUrl,
		ImageResize,
		ImageStyle,
		ImageTextAlternative,
		ImageToolbar,
		ImageUpload,
		ImageUtils,
		Indent,
		IndentBlock,
		Italic,
		Link,
		LinkImage,
		List,
		ListProperties,
		MediaEmbed,
		Mention,
		PageBreak,
		Paragraph,
		PasteFromOffice,
		PlainTableOutput,
		RemoveFormat,
		ShowBlocks,
		SourceEditing,
		SpecialCharacters,
		SpecialCharactersArrows,
		SpecialCharactersCurrency,
		SpecialCharactersEssentials,
		SpecialCharactersLatin,
		SpecialCharactersMathematical,
		SpecialCharactersText,
		Strikethrough,
		Subscript,
		Superscript,
		Table,
		TableCaption,
		TableCellProperties,
		TableColumnResize,
		TableLayout,
		TableProperties,
		TableToolbar,
		TextTransformation,
		TodoList,
		Underline
	],
	fontFamily: {
		supportAllValues: true
	},
	fontSize: {
		options: [10, 12, 14, 'default', 18, 20, 22],
		supportAllValues: true
	},
	fullscreen: {
		onEnterCallback: container =>
			container.classList.add(
				'editor-container',
				'editor-container_classic-editor',
				'editor-container_include-fullscreen',
				'main-container'
			)
	},
	heading: {
		options: [
			{
				model: 'paragraph',
				title: 'Paragraph',
				class: 'ck-heading_paragraph'
			},
			{
				model: 'heading1',
				view: 'h1',
				title: 'Heading 1',
				class: 'ck-heading_heading1'
			},
			{
				model: 'heading2',
				view: 'h2',
				title: 'Heading 2',
				class: 'ck-heading_heading2'
			},
			{
				model: 'heading3',
				view: 'h3',
				title: 'Heading 3',
				class: 'ck-heading_heading3'
			},
			{
				model: 'heading4',
				view: 'h4',
				title: 'Heading 4',
				class: 'ck-heading_heading4'
			},
			{
				model: 'heading5',
				view: 'h5',
				title: 'Heading 5',
				class: 'ck-heading_heading5'
			},
			{
				model: 'heading6',
				view: 'h6',
				title: 'Heading 6',
				class: 'ck-heading_heading6'
			}
		]
	},
	htmlSupport: {
		allow: [
			{
				name: /^.*$/,
				styles: true,
				attributes: true,
				classes: true
			}
		]
	},
	image: {
		toolbar: [
			'toggleImageCaption',
			'imageTextAlternative',
			'|',
			'imageStyle:inline',
			'imageStyle:wrapText',
			'imageStyle:breakText',
			'|',
			'resizeImage'
		]
	},
	language: 'zh',
	licenseKey: LICENSE_KEY,
	link: {
		addTargetToExternalLinks: true,
		defaultProtocol: 'https://',
		decorators: {
			toggleDownloadable: {
				mode: 'manual',
				label: 'Downloadable',
				attributes: {
					download: 'file'
				}
			}
		}
	},
	list: {
		properties: {
			styles: true,
			startIndex: true,
			reversed: true
		}
	},
	mention: {
		feeds: [
			{
				marker: '@',
				feed: [
					/* See: https://ckeditor.com/docs/ckeditor5/latest/features/mentions.html */
				]
			}
		]
	},
	menuBar: {
		isVisible: true
	},
	placeholder: '請在此輸入您的內容!',
	table: {
		contentToolbar: ['tableColumn', 'tableRow', 'mergeTableCells', 'tableProperties', 'tableCellProperties']
	}
};

ClassicEditor.create(document.querySelector('#editor'), editorConfig)
	.then(editor => {
		window.ckeditor = editor; // 讓全域可用
	})
	.catch(error => {
		console.error('CKEditor 啟動失敗', error);
	});

if (document.querySelector('#editor2')) {
	ClassicEditor.create(document.querySelector('#editor2'), editorConfig)
		.then(editor => {
			window.ckeditor2 = editor;
		})
		.catch(error => {
			console.error('CKEditor2 啟動失敗', error);
		});
}

// 初始化 CKEditor
//ClassicEditor
//	.create(document.querySelector('#editor'), {
//		licenseKey: 'eyJhbGciOiJFUzI1NiJ9.eyJleHAiOjE3ODc3MDIzOTksImp0aSI6ImJlNjYxZGMzLWE2MjMtNDc2NS04NjY2LTNkMTI0OWEwY2QwZiIsInVzYWdlRW5kcG9pbnQiOiJodHRwczovL3Byb3h5LWV2ZW50LmNrZWRpdG9yLmNvbSIsImRpc3RyaWJ1dGlvbkNoYW5uZWwiOlsiY2xvdWQiLCJkcnVwYWwiXSwiZmVhdHVyZXMiOlsiRFJVUCIsIkUyUCIsIkUyVyJdLCJ2YyI6IjAxNzRhMzE0In0.1lL04Wyy-qcTHGt81tE18PgI0xGv-qvo2kNPfzS1BobdLe2V00BhzOg-fP17kD9ZYXyZBMyoGv88EhYSyaI6XA',
//		// 這裡可以放設定，例如 toolbar
//		toolbar: [
//			'heading', '|',
//			'bold', 'italic', 'link', 'bulletedList', 'numberedList', '|',
//			'blockQuote', 'insertTable', 'undo', 'redo'
//		]

//	})
//	.then(editor => {
//		console.log('CKEditor 啟動成功', editor);
//	})
//	.catch(error => {
//		console.error('CKEditor 啟動失敗', error);
//	});

//// 初始化 CKEditor
//ClassicEditor
//	.create(document.querySelector('#editor2'), {
//		licenseKey: 'eyJhbGciOiJFUzI1NiJ9.eyJleHAiOjE3ODc3MDIzOTksImp0aSI6ImJlNjYxZGMzLWE2MjMtNDc2NS04NjY2LTNkMTI0OWEwY2QwZiIsInVzYWdlRW5kcG9pbnQiOiJodHRwczovL3Byb3h5LWV2ZW50LmNrZWRpdG9yLmNvbSIsImRpc3RyaWJ1dGlvbkNoYW5uZWwiOlsiY2xvdWQiLCJkcnVwYWwiXSwiZmVhdHVyZXMiOlsiRFJVUCIsIkUyUCIsIkUyVyJdLCJ2YyI6IjAxNzRhMzE0In0.1lL04Wyy-qcTHGt81tE18PgI0xGv-qvo2kNPfzS1BobdLe2V00BhzOg-fP17kD9ZYXyZBMyoGv88EhYSyaI6XA',
//		// 這裡可以放設定，例如 toolbar
//		toolbar: [
//			'heading', '|',
//			'bold', 'italic', 'link', 'bulletedList', 'numberedList', '|',
//			'blockQuote', 'insertTable', 'undo', 'redo'
//		]
//	})
//	.then(editor => {
//		console.log('CKEditor 啟動成功', editor);
//	})
//	.catch(error => {
//		console.error('CKEditor 啟動失敗', error);
//	});

