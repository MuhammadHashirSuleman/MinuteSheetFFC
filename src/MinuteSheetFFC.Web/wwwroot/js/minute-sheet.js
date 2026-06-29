window.minuteSheetEditor = {
    init: (id, dotNetRef, initialValue) => {
        if (!window.tinymce) {
            return false;
        }

        const selector = `#${id}`;
        const existing = window.tinymce.get(id);
        if (existing) {
            existing.remove();
        }

        window.tinymce.init({
            selector,
            height: 220,
            menubar: false,
            branding: false,
            promotion: false,
            plugins: "lists link table code",
            toolbar: "bold italic bullist numlist link | undo redo | removeformat code",
            skin: "oxide",
            content_css: "default",
            setup: editor => {
                editor.on("init", () => {
                    editor.setContent(initialValue || "");
                });

                editor.on("change keyup undo redo setcontent", () => {
                    dotNetRef.invokeMethodAsync("UpdateDescription", editor.getContent());
                });
            }
        });

        return true;
    },
    destroy: id => {
        if (!window.tinymce) {
            return;
        }

        const editor = window.tinymce.get(id);
        if (editor) {
            editor.remove();
        }
    }
};
