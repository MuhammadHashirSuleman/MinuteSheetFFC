window.minuteSheetEditor = (() => {
    const editors = new Map();

    const notifyChanged = entry => {
        entry.dotNetRef.invokeMethodAsync("UpdateDescription", entry.element.innerHTML);
    };

    const init = (id, dotNetRef, initialValue) => {
        const element = document.getElementById(id);
        if (!element) {
            return false;
        }

        destroy(id);
        element.innerHTML = initialValue || "";

        const entry = {
            element,
            dotNetRef,
            onInput: null,
            onPaste: null
        };

        entry.onInput = () => notifyChanged(entry);
        entry.onPaste = event => {
            event.preventDefault();
            const text = (event.clipboardData || window.clipboardData).getData("text/plain");
            document.execCommand("insertText", false, text);
            notifyChanged(entry);
        };

        element.addEventListener("input", entry.onInput);
        element.addEventListener("keyup", entry.onInput);
        element.addEventListener("paste", entry.onPaste);

        editors.set(id, entry);
        return true;
    };

    const command = (id, commandName) => {
        const entry = editors.get(id);
        if (!entry) {
            return;
        }

        entry.element.focus();
        document.execCommand(commandName, false, null);
        notifyChanged(entry);
    };

    const setHtml = (id, value) => {
        const entry = editors.get(id);
        if (!entry) {
            return;
        }

        entry.element.innerHTML = value || "";
        notifyChanged(entry);
    };

    const destroy = id => {
        const entry = editors.get(id);
        if (!entry) {
            return;
        }

        entry.element.removeEventListener("input", entry.onInput);
        entry.element.removeEventListener("keyup", entry.onInput);
        entry.element.removeEventListener("paste", entry.onPaste);
        editors.delete(id);
    };

    return {
        init,
        command,
        setHtml,
        destroy
    };
})();
