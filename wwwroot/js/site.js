// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", () => {
  // サイドバー切替
  const toggle = document.getElementById("sidebarToggle");

  if (toggle) {
    toggle.addEventListener("click", () => {
      document.documentElement.classList.toggle("sidebar-collapsed");

      localStorage.setItem(
        "sidebar",
        document.documentElement.classList.contains("sidebar-collapsed")
          ? "closed"
          : "open",
      );
    });
  }

  // 検索条件リセット
  const resetButton = document.getElementById("resetSearch");

  if (resetButton) {
    resetButton.addEventListener("click", () => {
      const form = resetButton.closest("form");

      if (!form) return;

      form.reset();
      form.submit();
    });
  }
});
