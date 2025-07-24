document.addEventListener("DOMContentLoaded", () => {
    const input = document.getElementById("text-message");
    const mentionBox = document.getElementById("mention-box");
    const mentionGemini = document.getElementById("mention-gemini");

    // Hiện box nếu input là "@"
    input.addEventListener("input", () => {
        const val = input.value.trim();
        if (val === "@") {
            showMentionBox();
        } else {
            hideMentionBox();
        }
    });

    // Xử lý phím
    input.addEventListener("keydown", (e) => {
        if (mentionBox.classList.contains("hidden")) return;

        if (e.key === "Enter") {
            e.preventDefault();
            insertGeminiMention();
        } else if (!["@", "ArrowDown", "ArrowUp"].includes(e.key)) {
            hideMentionBox();
        }
    });

    // Click chọn Gemini
    mentionGemini.addEventListener("click", () => {
        insertGeminiMention();
    });

    // Insert mention
    function insertGeminiMention() {
        input.value = "@Gemini ";
        hideMentionBox();
        input.focus();
    }

    function showMentionBox() {
        mentionBox.classList.remove("hidden");
    }

    function hideMentionBox() {
        mentionBox.classList.add("hidden");
    }
});
