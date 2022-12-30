export default function savePdf(blob, filename) {
  const objectData = window.URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = objectData;
  link.download = filename;
  link.click();
  setTimeout(function () {
    // For Firefox it is necessary to delay revoking the ObjectURL
    window.URL.revokeObjectURL(objectData);
  }, 600);
}
