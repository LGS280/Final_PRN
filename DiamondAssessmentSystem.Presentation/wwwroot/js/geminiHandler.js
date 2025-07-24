document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("sendMessageForm");
    const input = document.getElementById("text-message");
    const conversationId = document.querySelector("input[name='conversationId']").value;
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    form.addEventListener("submit", async function (e) {
        const raw = input.value.trim();

        if (raw.toLowerCase().startsWith("@gemini")) {
            e.preventDefault();
            const query = raw.slice(7).trim();
            if (!query) return;

            try {
                // Gọi API Gemini
                const res = await fetch("/api/AI/ask", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(raw)
                });

                if (!res.ok) {
                    console.error("Gemini API failed");
                    return;
                }

                const message = await res.text();

                // Gửi ngay lên giao diện bằng SignalR
                const payload = {
                    chatId: 0,
                    conversationId: parseInt(conversationId),
                    senderId: 0,
                    senderName: "Gemini",
                    senderRole: "consultant",
                    messageType: 0,
                    message,
                    filePath: null,
                    fileName: null,
                    fileSize: null,
                    sentAt: new Date().toISOString()
                };

                connection.invoke("SendToConversation", conversationId.toString(), payload);
                input.value = "";

                // Gửi về server để lưu DB
                await fetch(`/ChatMessages/SendGeminiMessage?conversationId=${conversationId}`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "RequestVerificationToken": token
                    },
                    body: JSON.stringify(message)
                });

            } catch (err) {
                console.error("Gemini error:", err);
            }
        }
    });
});
