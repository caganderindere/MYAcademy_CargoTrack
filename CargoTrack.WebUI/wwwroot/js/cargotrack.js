(function () {
  "use strict";

  const brand = { accent: "#F26414", primary: "#1546C4", ink: "#0B1B33", teal: "#0EA5A0", line: "#E4E9F2", muted: "#8A94A6" };
  const statusPalette = { 1: "#94A3B8", 2: "#1546C4", 3: "#5B3FD1", 4: "#0EA5A0", 5: "#F26414", 6: "#16A34A", 7: "#DC2626", 8: "#D97706", 9: "#334155" };

  document.addEventListener("click", function (e) {
    const toggle = e.target.closest("[data-sidebar-toggle]");
    if (toggle) {
      document.querySelector(".panel")?.classList.toggle("sb-open");
    }
    if (e.target.closest(".sidebar-backdrop")) {
      document.querySelector(".panel")?.classList.remove("sb-open");
    }
  });

  document.addEventListener("submit", function (e) {
    const form = e.target;
    const message = form.getAttribute("data-confirm");
    if (message && !window.confirm(message)) {
      e.preventDefault();
    }
  });

  document.querySelectorAll("[data-autohide]").forEach(function (el) {
    setTimeout(function () { el.style.transition = "opacity .4s"; el.style.opacity = "0"; setTimeout(function () { el.remove(); }, 450); }, 6000);
  });

  function readJson(id) {
    const el = document.getElementById(id);
    return el ? JSON.parse(el.textContent) : null;
  }

  function renderCharts() {
    if (typeof Chart === "undefined") {
      return;
    }
    Chart.defaults.font.family = getComputedStyle(document.body).fontFamily;
    Chart.defaults.color = brand.muted;

    document.querySelectorAll("canvas[data-chart]").forEach(function (canvas) {
      const cfg = readJson(canvas.getAttribute("data-chart"));
      if (!cfg) {
        return;
      }
      const ctx = canvas.getContext("2d");

      if (cfg.type === "line") {
        const gradient = ctx.createLinearGradient(0, 0, 0, canvas.parentElement.clientHeight || 280);
        gradient.addColorStop(0, "rgba(242,100,20,.28)");
        gradient.addColorStop(1, "rgba(242,100,20,0)");
        new Chart(ctx, {
          type: "line",
          data: { labels: cfg.labels, datasets: [{ label: cfg.label || "", data: cfg.values, borderColor: brand.accent, backgroundColor: gradient, fill: true, tension: .35, cubicInterpolationMode: "monotone", borderWidth: 3, pointRadius: 4, pointBackgroundColor: "#fff", pointBorderColor: brand.accent, pointBorderWidth: 2 }] },
          options: { maintainAspectRatio: false, plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true, ticks: { precision: 0 }, grid: { color: "#EEF2F7" } }, x: { grid: { display: false } } } }
        });
      }

      if (cfg.type === "doughnut") {
        new Chart(ctx, {
          type: "doughnut",
          data: { labels: cfg.labels, datasets: [{ data: cfg.values, backgroundColor: cfg.keys.map(function (k) { return statusPalette[k] || brand.muted; }), borderWidth: 3, borderColor: "#fff", hoverOffset: 6 }] },
          options: { maintainAspectRatio: false, cutout: "68%", plugins: { legend: { position: "bottom", labels: { usePointStyle: true, boxWidth: 8, padding: 14 } } } }
        });
      }

      if (cfg.type === "bar") {
        new Chart(ctx, {
          type: "bar",
          data: { labels: cfg.labels, datasets: [{ label: cfg.label || "", data: cfg.values, backgroundColor: brand.primary, borderRadius: 8, maxBarThickness: 34 }] },
          options: { maintainAspectRatio: false, indexAxis: cfg.horizontal ? "y" : "x", plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true, ticks: { precision: 0 }, grid: { color: "#EEF2F7" } }, x: { grid: { display: false } } } }
        });
      }
    });
  }

  function renderCodes() {
    document.querySelectorAll("[data-qr]").forEach(function (el) {
      if (typeof QRCode !== "undefined") {
        new QRCode(el, { text: el.getAttribute("data-qr"), width: 116, height: 116, colorDark: brand.ink, colorLight: "#ffffff" });
      }
    });
    document.querySelectorAll("svg[data-barcode]").forEach(function (el) {
      if (typeof JsBarcode !== "undefined") {
        JsBarcode(el, el.getAttribute("data-barcode"), { format: "CODE128", lineColor: brand.ink, height: 56, displayValue: true, fontSize: 13, margin: 0, font: "monospace" });
      }
    });
  }

  function initStatusForm() {
    const form = document.querySelector("[data-status-form]");
    if (!form) {
      return;
    }
    const select = form.querySelector("[name=NewStatus]");
    const branch = form.querySelector("[name=BranchId]");
    const centerWrap = form.querySelector("[data-center-wrap]");
    const origin = form.getAttribute("data-origin");
    const destination = form.getAttribute("data-destination");

    function sync() {
      const value = parseInt(select.value, 10);
      centerWrap.classList.toggle("d-none", value !== 3);
      if (value === 2) { branch.value = origin; }
      else if (value === 4 || value === 5) { branch.value = destination; }
      else { branch.value = ""; }
    }
    select.addEventListener("change", sync);
    sync();
  }

  function debounce(fn, wait) {
    let t;
    return function () { const args = arguments; clearTimeout(t); t = setTimeout(function () { fn.apply(null, args); }, wait); };
  }

  function initCargoCreate() {
    const form = document.querySelector("[data-cargo-create]");
    if (!form) {
      return;
    }
    const searchUrl = form.getAttribute("data-search-url");
    const addressUrl = form.getAttribute("data-address-url");
    const quoteUrl = form.getAttribute("data-quote-url");

    form.querySelectorAll("[data-party]").forEach(function (box) {
      const input = box.querySelector("[data-party-search]");
      const results = box.querySelector(".search-results");
      const hidden = box.querySelector("input[type=hidden]");
      const chosen = box.querySelector("[data-party-chosen]");
      const role = box.getAttribute("data-party");

      input.addEventListener("input", debounce(function () {
        const term = input.value.trim();
        if (term.length < 2) { results.classList.add("d-none"); return; }
        fetch(searchUrl + "?term=" + encodeURIComponent(term), { headers: { "X-Requested-With": "XMLHttpRequest" } })
          .then(function (r) { return r.json(); })
          .then(function (items) {
            results.innerHTML = "";
            if (!items.length) {
              results.innerHTML = '<div class="p-3 text-muted-ct small">Sonuç bulunamadı.</div>';
            }
            items.forEach(function (c) {
              const b = document.createElement("button");
              b.type = "button";
              b.innerHTML = '<div class="fw-750"></div><div class="small text-muted-ct"></div>';
              b.children[0].textContent = c.fullName;
              b.children[1].textContent = c.phoneNumber;
              b.addEventListener("click", function () {
                hidden.value = c.id;
                chosen.querySelector("[data-name]").textContent = c.fullName;
                chosen.querySelector("[data-phone]").textContent = c.phoneNumber;
                chosen.classList.remove("d-none");
                input.value = "";
                results.classList.add("d-none");
                if (role === "receiver") { loadAddresses(c.id); }
              });
              results.appendChild(b);
            });
            results.classList.remove("d-none");
          });
      }, 250));

      chosen.querySelector("[data-clear]").addEventListener("click", function () {
        hidden.value = "";
        chosen.classList.add("d-none");
        if (role === "receiver") { loadAddresses(null); }
      });
    });

    const addressSelect = form.querySelector("[name=DeliveryAddressId]");
    function loadAddresses(customerId) {
      addressSelect.innerHTML = '<option value="">Önce alıcı seçin</option>';
      if (!customerId) { return; }
      fetch(addressUrl + "?customerId=" + customerId, { headers: { "X-Requested-With": "XMLHttpRequest" } })
        .then(function (r) { return r.json(); })
        .then(function (items) {
          addressSelect.innerHTML = items.length ? '<option value="">Adres seçin</option>' : '<option value="">Alıcının kayıtlı adresi yok</option>';
          items.forEach(function (a) {
            const o = document.createElement("option");
            o.value = a.id;
            o.textContent = a.title + " — " + a.district + " / " + a.cityName;
            addressSelect.appendChild(o);
          });
        });
    }

    const destinationSelect = form.querySelector("select[name=DestinationBranchId]");
    addressSelect.addEventListener("change", function () {
      const text = addressSelect.options[addressSelect.selectedIndex]?.textContent || "";
      const city = text.split("/").pop().trim();
      if (!city || !destinationSelect) { return; }
      const match = Array.from(destinationSelect.options).find(function (o) { return o.textContent.trim().endsWith("(" + city + ")"); });
      if (match) {
        destinationSelect.value = match.value;
        destinationSelect.dispatchEvent(new Event("change"));
      }
    });

    const servicesHidden = form.querySelector("[name=AdditionalServices]");
    form.querySelectorAll("[data-service]").forEach(function (cb) {
      cb.addEventListener("change", function () {
        let total = 0;
        form.querySelectorAll("[data-service]:checked").forEach(function (x) { total += parseInt(x.value, 10); });
        servicesHidden.value = total;
        requestQuote();
      });
    });

    const quoteBox = document.querySelector("[data-quote]");
    const requestQuote = debounce(function () {
      const data = new FormData(form);
      const ready = data.get("OriginBranchId") && data.get("DestinationBranchId") && parseFloat(data.get("Weight")) > 0 && parseFloat(data.get("Width")) > 0 && parseFloat(data.get("Length")) > 0 && parseFloat(data.get("Height")) > 0;
      if (!ready || !quoteBox) { return; }
      quoteBox.classList.add("opacity-50");
      fetch(quoteUrl, { method: "POST", body: data, headers: { "X-Requested-With": "XMLHttpRequest" } })
        .then(function (r) { return r.json().then(function (j) { return { ok: r.ok, body: j }; }); })
        .then(function (res) {
          quoteBox.classList.remove("opacity-50");
          const err = quoteBox.querySelector("[data-quote-error]");
          if (!res.ok) {
            err.textContent = res.body.message || "Fiyat hesaplanamadı.";
            err.classList.remove("d-none");
            return;
          }
          err.classList.add("d-none");
          quoteBox.querySelector("[data-quote-price]").textContent = res.body.price.toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + " ₺";
          quoteBox.querySelector("[data-quote-desi]").textContent = res.body.desi.toLocaleString("tr-TR");
          quoteBox.querySelector("[data-quote-date]").textContent = new Date(res.body.estimatedDeliveryDate).toLocaleString("tr-TR", { day: "2-digit", month: "long", weekday: "long", hour: "2-digit", minute: "2-digit" });
        })
        .catch(function () { quoteBox.classList.remove("opacity-50"); });
    }, 350);

    form.querySelectorAll("[data-quote-field]").forEach(function (el) {
      el.addEventListener("input", requestQuote);
      el.addEventListener("change", requestQuote);
    });
  }

  document.addEventListener("DOMContentLoaded", function () {
    renderCharts();
    renderCodes();
    initStatusForm();
    initCargoCreate();
  });
})();
