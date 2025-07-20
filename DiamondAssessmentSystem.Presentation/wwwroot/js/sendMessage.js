document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("sendMessageForm");
    const textInput = document.getElementById("text-message");
    const fileInput = document.getElementById("file-upload");
    const progressBar = document.getElementById("upload-progress");
    const emojiPickerContainer = document.getElementById("emoji-picker");

    if (!form || !textInput || !fileInput || !emojiPickerContainer) return;

    const token = form.querySelector('input[name="__RequestVerificationToken"]').value;
    const conversationIdValue = form.querySelector('input[name="conversationId"]').value;

    // Gửi tin nhắn văn bản
    form.addEventListener("submit", async (e) => {
        e.preventDefault();
        const message = textInput.value.trim();
        if (!message) return;

        const formData = new FormData();
        formData.append("Message", message);
        formData.append("conversationId", conversationIdValue);
        formData.append("__RequestVerificationToken", token);

        try {
            const res = await fetch(`/ChatMessages/SendMessage?conversationId=${conversationIdValue}`, {
                method: "POST",
                body: formData
            });

            if (res.ok) {
                form.reset();
            } else {
                alert("Failed to send message.");
            }
        } catch (err) {
            console.error("Text message error:", err);
            alert("Connection error.");
        }
    });

    // Upload file kèm progress
    fileInput.addEventListener("change", () => {
        if (fileInput.files.length === 0) return;

        const file = fileInput.files[0];
        const formData = new FormData();
        formData.append("file", file);
        formData.append("conversationId", conversationIdValue);
        formData.append("__RequestVerificationToken", token);

        const xhr = new XMLHttpRequest();
        xhr.open("POST", `/ChatMessages/UploadFile?conversationId=${conversationIdValue}`, true);

        xhr.upload.onprogress = (e) => {
            if (e.lengthComputable) {
                const percent = Math.round((e.loaded / e.total) * 100);
                progressBar.classList.remove("hidden");
                progressBar.value = percent;
            }
        };

        xhr.onload = () => {
            progressBar.classList.add("hidden");
            progressBar.value = 0;
            if (xhr.status === 200) {
                form.reset();
            } else {
                alert("Upload failed.");
            }
        };

        xhr.onerror = () => {
            alert("Upload error.");
            progressBar.classList.add("hidden");
            progressBar.value = 0;
        };

        xhr.send(formData);
    });

    // Emoji picker
    const picker = new EmojiButton({
        position: 'top-end',
        theme: 'light'
    });

    picker.on('emoji', emoji => {
        textInput.value += emoji;
        textInput.focus();
    });

    const emojiBtn = document.createElement("button");
    emojiBtn.type = "button";
    emojiBtn.innerText = "😊";
    emojiBtn.title = "Insert emoji";
    emojiBtn.className = "text-xl px-3 py-2 rounded-full bg-gray-100 hover:bg-gray-200 transition shadow-sm";
    emojiBtn.addEventListener("click", () => picker.togglePicker(emojiBtn));

    emojiPickerContainer.appendChild(emojiBtn);
});
