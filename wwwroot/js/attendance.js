function openAttendanceModal(date, clockIn, clockOut, status, breakMinutes) {
  document.getElementById("attendanceDate").value = date;
  document.getElementById("displayDate").value = date;

  // clockIn = "09:30" のような文字列
  if (clockIn) {
    const [inHour, inMin] = clockIn.split(":");
    document.getElementById("clockInHour").value = inHour;
    document.getElementById("clockInMinute").value = inMin;
  } else {
    document.getElementById("clockInHour").value = "";
    document.getElementById("clockInMinute").value = "";
  }

  if (clockOut) {
    const [outHour, outMin] = clockOut.split(":");
    document.getElementById("clockOutHour").value = outHour;
    document.getElementById("clockOutMinute").value = outMin;
  } else {
    document.getElementById("clockOutHour").value = "";
    document.getElementById("clockOutMinute").value = "";
  }

  // 休憩時間（分 → 時・分に変換）
  const breakH = Math.floor((breakMinutes ?? 0) / 60);
  const breakM = (breakMinutes ?? 0) % 60;

  document.getElementById("breakHour").value = breakH;
  document.getElementById("breakMinute").value = breakM;

  document.getElementById("status").value = status;

  const modal = new bootstrap.Modal(document.getElementById("attendanceModal"));
  modal.show();
}
