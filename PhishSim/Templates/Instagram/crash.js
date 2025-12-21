document.querySelectorAll(".crash-link").forEach(link => {
    link.addEventListener("click", e => {
        e.preventDefault();

        // loading state (real apps often hang briefly)
        document.body.innerHTML = `
      <div style="
        height:100vh;
        display:flex;
        align-items:center;
        justify-content:center;
        font-family:system-ui,-apple-system,BlinkMacSystemFont;
        background:#ffffff;
        color:#111;
        font-size:15px;
      ">
        Loading…
      </div>
    `;

        setTimeout(() => {
            document.body.innerHTML = `
        <div style="
          min-height:100vh;
          padding:64px 48px;
          font-family:system-ui,-apple-system,BlinkMacSystemFont;
          background:#ffffff;
          color:#111;
        ">
          <div style="
            max-width:720px;
          ">
            <div style="
              font-size:72px;
              font-weight:700;
              letter-spacing:-1px;
              margin-bottom:24px;
            ">
              500
            </div>

            <div style="
              font-size:22px;
              font-weight:500;
              margin-bottom:12px;
            ">
              Internal Server Error
            </div>

            <div style="
              font-size:16px;
              line-height:1.6;
              color:#444;
              max-width:560px;
            ">
              The server encountered an unexpected condition that prevented it
              from fulfilling the request.
            </div>

            <div style="
              margin-top:32px;
              font-size:14px;
              color:#888;
            ">
              Request ID: ${Math.random().toString(36).slice(2, 10)}
            </div>
          </div>
        </div>
      `;
        }, 900);
    });
});