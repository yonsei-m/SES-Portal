document.addEventListener("DOMContentLoaded", function () {
  // チャットを最新位置へスクロール
  const thread = document.querySelector(".chat-thread");
  if (thread) {
    thread.scrollTop = thread.scrollHeight;
  }
  // Enter送信
  const input = document.querySelector(".chat-input textarea");
  if (input) {
    input.addEventListener("keydown", function (e) {
      // Enterのみの場合送信
      if (e.key === "Enter" && !e.shiftKey) {
        e.preventDefault();
        this.closest("form").requestSubmit();
      }
    });
  }
  const form = document.querySelector(".chat-input");
  if (form) {
    form.addEventListener("submit", function (e) {
      e.preventDefault();
      sendMessage();
    });
  }
  // SignalR接続
  const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

  connection
    .start()
    .then(async function () {
      const userId = document.getElementById("currentUserId")?.value;
      await connection.invoke("JoinUser", userId);
      const roomId = document.getElementById("roomId").value;
      if (roomId) {
        await connection.invoke("JoinRoom", roomId.toString());
      }
    })

    .catch(function (err) {
      console.error(err);
    });

  connection.on(
    "ReceiveMessage",
    function (
      roomId,
      senderId,
      senderName,
      message,
      fileName,
      filePath,
      fileSize,
      contentType,
      sentAt,
      unreadCount,
    ) {
      const currentRoomId = document.getElementById("roomId").value;

      // 開いているトーク以外は表示しない
      if (currentRoomId != roomId) {
        updateRoomList(roomId, message, sentAt, false);
        return;
      }

      const thread = document.querySelector(".chat-thread");
      const currentUserId = document.getElementById("currentUserId")?.value;
      const mine = senderId === currentUserId;

      const html = `
            <div class="message-row ${mine ? "mine" : "other"}">
                <div class="message-bubble">

                    ${
                      message
                        ? `<div class="message-text">${message}</div>`
                        : ""
                    }

                    ${
                      filePath
                        ? `<div class="chat-file">
                              <i class="bi bi-paperclip"></i>
                              <a href="${filePath}" target="_blank">
                                  ${fileName}
                              </a>
                          </div>`
                        : ""
                    }
                </div>
                  <div class="message-time">
                      ${new Date(sentAt).toLocaleTimeString([], {
                        hour: "2-digit",
                        minute: "2-digit",
                      })}
                      ${mine ? '<span class="read-status"></span>' : ""}
                  </div>
            </div>`;

      thread.insertAdjacentHTML("beforeend", html);

      thread.scrollTop = thread.scrollHeight;
      updateRoomList(roomId, message, sentAt, mine);

      if (filePath) {
        const sharedFiles = document.getElementById("profileSharedFilesList");

        if (sharedFiles) {
          const emptyMessage = sharedFiles.querySelector(".empty-message");

          if (emptyMessage) {
            emptyMessage.remove();
          }
          const fileHtml = `
                    <a href="${filePath}" target="_blank" class="shared-file">
                        <div class="shared-file-icon">
                            <i class="bi bi-file-earmark"></i>
                        </div>

                        <div class="shared-file-body">
                            <div class="shared-file-name">
                                ${fileName}
                            </div>
                            <div class="shared-file-info">
                                ${Math.round(fileSize / 1024)} KB
                            </div>
                        </div>
                    </a>`;

          sharedFiles.insertAdjacentHTML("afterbegin", fileHtml);
        }
      }

      if (currentRoomId == roomId && senderId != currentUserId) {
        fetch("?handler=Read", {
          method: "POST",
          headers: {
            RequestVerificationToken: token,
            "Content-Type": "application/x-www-form-urlencoded",
          },
          body: `roomId=${roomId}`,
        });
      }
    },
  );
  const token = document.querySelector(
    'input[name="__RequestVerificationToken"]',
  ).value;

  connection.on("ReceiveRead", function (roomId, readerId) {
    const currentRoomId = document.getElementById("roomId")?.value;

    const currentUserId = "@Model.CurrentUserId";

    if (currentRoomId != roomId) {
      return;
    }

    // 自分自身の既読通知は無視
    if (readerId === currentUserId) {
      return;
    }

    const rows = document.querySelectorAll(".message-row.mine");

    rows.forEach((row) => {
      const status = row.querySelector(".read-status");

      if (status) {
        status.textContent = "既読";
      }
    });
  });
  async function sendMessage() {
    const textarea = document.querySelector(".message-textarea");
    const message = textarea.value.trim();
    const hasFile = attachmentInput.files.length > 0;

    if (message === "" && !hasFile) return;
    const formData = new FormData();
    formData.append("Message", message);
    if (attachmentInput.files.length > 0) {
      formData.append("Attachment", attachmentInput.files[0]);
    }
    const employeeId = document.getElementById("employeeId")?.value;
    const response = await fetch(`?handler=Send&id=${employeeId}`, {
      method: "POST",
      headers: { RequestVerificationToken: token },
      body: formData,
    });
    if (!response.ok) {
      console.error("送信失敗");
      return;
    }

    textarea.value = "";
    textarea.style.height = "auto";
    attachmentInput.value = "";
    selectedFile.style.display = "none";
    selectedFileName.textContent = "";
    textarea.focus();
  }

  function updateRoomList(roomId, message, sentAt, mine) {
    const room = document.querySelector(
      `.chat-room-item[data-room-id="${roomId}"]`,
    );

    if (!room) return;

    // 最新メッセージ
    const messageElement = room.querySelector(".room-message");

    if (messageElement) messageElement.textContent = message;

    // 時刻
    const timeElement = room.querySelector(".room-time");

    if (timeElement) {
      timeElement.textContent = new Date(sentAt).toLocaleTimeString([], {
        hour: "2-digit",
        minute: "2-digit",
      });
    }

    // 一覧の先頭へ移動
    const list = document.querySelector(".chat-room-list");

    list.prepend(room);

    if (!mine) {
      let badge = room.querySelector(".room-unread");

      if (!badge) {
        badge = document.createElement("span");
        badge.className = "badge bg-primary room-unread";
        room.querySelector(".room-bottom").appendChild(badge);
        badge.textContent = "1";
      } else {
        badge.textContent = Number(badge.textContent) + 1;
      }
    }
  }

  const attachButton = document.getElementById("attachButton");
  const attachmentInput = document.getElementById("attachmentInput");
  const selectedFile = document.getElementById("selectedFile");
  const selectedFileName = document.getElementById("selectedFileName");
  attachButton.addEventListener("click", function () {
    attachmentInput.click();
  });

  attachmentInput.addEventListener("change", function () {
    if (this.files.length === 0) {
      selectedFile.style.display = "none";

      return;
    }

    selectedFile.style.display = "flex";

    selectedFileName.textContent = this.files[0].name;
  });
  const profilePanels = [
    "profilePanel",
    "filesPanel",
    "memoListPanel",
    "memoEditPanel",
  ].map((id) => document.getElementById(id));

  function hideAllProfilePanels() {
    profilePanels.forEach((panel) => {
      if (panel) panel.style.display = "none";
    });
  }
  document
    .getElementById("showAllFilesBtn")
    ?.addEventListener("click", function (e) {
      e.preventDefault();

      profilePanel.style.display = "none";
      filesPanel.style.display = "block";
    });

  document
    .getElementById("backProfileBtn")
    ?.addEventListener("click", function (e) {
      e.preventDefault();

      hideAllProfilePanels();

      profilePanel.style.display = "block";
    });

  const createBtn = document.getElementById("createMemoBtn");
  const backBtn = document.getElementById("backMemoBtn");
  const memoCards = document.querySelectorAll(".chat-memo-card");
  const showAllMemosBtn = document.getElementById("showAllMemosBtn");
  const backMemoListBtn = document.getElementById("backMemoListBtn");

  let memoEditFromList = false;

  showAllMemosBtn?.addEventListener("click", (e) => {
    e.preventDefault();
    hideAllProfilePanels();
    memoListPanel.style.display = "block";
  });

  backMemoListBtn?.addEventListener("click", (e) => {
    e.preventDefault();
    hideAllProfilePanels();
    profilePanel.style.display = "block";
  });

  if (createBtn) {
    createBtn.addEventListener("click", function (e) {
      e.preventDefault();
      memoEditFromList = false;
      openMemoEditor(null);
    });
  }

  function openMemoEditor(card) {
    document.getElementById("memoId").value = card?.dataset.id ?? "";

    document.getElementById("memoTitle").value = card?.dataset.title ?? "";

    document.getElementById("memoContent").value = card?.dataset.content ?? "";

    hideAllProfilePanels();
    memoEditPanel.style.display = "block";
  }

  memoCards.forEach((card) => {
    card.addEventListener("click", function () {
      memoEditFromList = this.dataset.source === "list";
      openMemoEditor(this);
    });
  });

  if (backBtn) {
    backBtn.addEventListener("click", function (e) {
      e.preventDefault();
      hideAllProfilePanels();
      if (memoEditFromList) {
        memoListPanel.style.display = "block";
      } else {
        profilePanel.style.display = "block";
      }
    });
  }
});
