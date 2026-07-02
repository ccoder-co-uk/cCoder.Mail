window.MailGrids = {
    apiRoot: "/Api/Core",
    initialized: false,

    configs: {
        MailServer: {
            name: "MailServer",
            title: "Mail Server",
            key: "Id",
            fields: {
                Id: { label: "Id", readonly: true, create: false, type: "number" },
                AppId: { label: "App Id", type: "number" },
                Name: { label: "Name" },
                User: { label: "User" },
                Password: { label: "Password", type: "password" },
                Host: { label: "Host" },
                FromEmail: { label: "From Email" },
                Port: { label: "Port", type: "number" },
                EnableSSL: { label: "Enable SSL", type: "checkbox" }
            },
            columns: ["Name", "Host", "Port", "EnableSSL", "User", "FromEmail", "AppId", "Id"]
        },
        QueuedEmail: {
            name: "QueuedEmail",
            title: "Queued Email",
            key: "Id",
            expand: "FailedSends",
            fields: {
                Id: { label: "Id", readonly: true, create: false, type: "number" },
                AppId: { label: "App Id", type: "number" },
                MailServerName: { label: "Mail Server" },
                SentByUserId: { label: "Sent By" },
                To: { label: "To" },
                CC: { label: "CC" },
                Subject: { label: "Subject" },
                Content: { label: "Content", type: "textarea" },
                IsBodyHtml: { label: "Is Body HTML", type: "checkbox" }
            },
            columns: ["To", "Subject", "MailServerName", "SentByUserId", "IsBodyHtml", "Id"]
        },
        SentEmail: {
            name: "SentEmail",
            title: "Sent Email",
            key: "Id",
            fields: {
                Id: { label: "Id", readonly: true, create: false, type: "number" },
                AppId: { label: "App Id", type: "number" },
                SentByUserId: { label: "Sent By" },
                From: { label: "From" },
                To: { label: "To" },
                CC: { label: "CC" },
                Subject: { label: "Subject" },
                Content: { label: "Content", type: "textarea" },
                SentOn: { label: "Sent On" },
                IsBodyHtml: { label: "Is Body HTML", type: "checkbox" }
            },
            columns: ["To", "Subject", "From", "SentOn", "SentByUserId", "Id"]
        },
        EmailSendFailure: {
            name: "EmailSendFailure",
            title: "Send Failure",
            key: "Id",
            readonly: true,
            fields: {
                Id: { label: "Id", readonly: true, create: false, type: "number" },
                EmailId: { label: "Queued Email Id", readonly: true, type: "number" },
                AttemptedOn: { label: "Attempted On", readonly: true },
                FailureReason: { label: "Failure Reason", readonly: true, type: "textarea" }
            },
            columns: ["AttemptedOn", "FailureReason", "Id"]
        },
        ReceivedEmail: {
            name: "ReceivedEmail",
            title: "Received Email",
            key: "MessageId",
            readonly: true,
            fields: {
                MessageId: { label: "Message Id", readonly: true },
                From: { label: "From", readonly: true },
                To: { label: "To", readonly: true },
                CC: { label: "CC", readonly: true },
                Subject: { label: "Subject", readonly: true },
                Content: { label: "Content", readonly: true, type: "textarea" },
                IsBodyHtml: { label: "Is Body HTML", readonly: true, type: "checkbox" },
                ReceivedOn: { label: "Received On", readonly: true }
            },
            columns: ["ReceivedOn", "From", "To", "Subject", "MessageId"]
        }
    },

    store: {},

    init: function () {
        if (this.initialized || !MailApi.isAuthenticated()) {
            return;
        }

        this.initialized = true;
        document.getElementById("create-mail-server")
            ?.addEventListener("click", () => this.openEditor(this.configs.MailServer, null, null));
        document.getElementById("create-queued-email")
            ?.addEventListener("click", () => this.openEditor(this.configs.QueuedEmail, null, null));
        document.getElementById("create-sent-email")
            ?.addEventListener("click", () => this.openEditor(this.configs.SentEmail, null, null));
        document.getElementById("receive-email")
            ?.addEventListener("click", () => this.receiveEmails());
        document.querySelectorAll("[data-main-tab]").forEach(tab =>
            tab.addEventListener("click", () => this.showMainTab(tab.dataset.mainTab)));

        this.loadAll();
    },

    loadAll: async function () {
        await this.loadMailServers();
        await this.loadQueuedEmails();
        await this.loadSentEmails();
        this.renderGrid("received-email-grid", this.configs.ReceivedEmail, [], "ReceivedEmail");
    },

    loadMailServers: async function () {
        try {
            const rows = await this.read(this.configs.MailServer);
            this.renderMailServerGrid(rows);
            MailApi.notify("Ready");
        } catch (error) {
            MailApi.notify(error.message, true);
        }
    },

    loadQueuedEmails: async function () {
        try {
            const rows = await this.read(this.configs.QueuedEmail);
            this.renderGrid("queued-email-grid", this.configs.QueuedEmail, rows, "QueuedEmail");
            MailApi.notify("Ready");
        } catch (error) {
            MailApi.notify(error.message, true);
        }
    },

    loadSentEmails: async function () {
        try {
            const rows = await this.read(this.configs.SentEmail);
            this.renderGrid("sent-email-grid", this.configs.SentEmail, rows, "SentEmail");
            MailApi.notify("Ready");
        } catch (error) {
            MailApi.notify(error.message, true);
        }
    },

    receiveEmails: async function () {
        try {
            const rows = await MailApi.post(
                `${this.apiRoot}/ReceivedEmail/Receive`,
                this.receiveRequest());

            this.renderGrid("received-email-grid", this.configs.ReceivedEmail, rows ?? [], "ReceivedEmail");
            MailApi.notify(`${rows?.length ?? 0} received emails loaded`);
        } catch (error) {
            MailApi.notify(error.message, true);
        }
    },

    receiveRequest: function () {
        return {
            user: document.getElementById("receive-user")?.value,
            from: this.dateInputValue("receive-from"),
            to: this.dateInputValue("receive-to"),
            maximumMessages: Number(document.getElementById("receive-maximum")?.value || 100)
        };
    },

    read: async function (config, context) {
        let url = `${this.apiRoot}/${config.name}?$top=500`;
        const filters = this.filtersFor(config, context);

        if (filters.length > 0) {
            url += `&$filter=${encodeURIComponent(filters.join(" and "))}`;
        }

        if (config.expand) {
            url += `&$expand=${encodeURIComponent(config.expand)}`;
        }

        const body = await MailApi.get(url);
        return MailApi.unwrapCollection(body);
    },

    filtersFor: function (config, context) {
        if (!context?.mailServer) {
            return [];
        }

        const server = context.mailServer;

        if (config.name === "QueuedEmail") {
            return [
                `AppId eq ${server.AppId}`,
                `MailServerName eq '${this.odataString(server.Name)}'`
            ];
        }

        if (config.name === "SentEmail") {
            return [`AppId eq ${server.AppId}`];
        }

        return [];
    },

    renderMailServerGrid: function (rows) {
        this.renderGrid("mail-server-grid", this.configs.MailServer, rows, "MailServer");
    },

    renderGrid: function (elementId, config, rows, scope, context = null) {
        const grid = document.getElementById(elementId);
        grid.innerHTML = this.tableHtml(config, rows, scope, context);
        this.bindGridActions(grid);
    },

    tableHtml: function (config, rows, scope, context = null) {
        const headers = [
            `<th class="mail-expand-column"></th>`,
            ...config.columns.map(column => `<th>${this.escape(this.label(config, column))}</th>`),
            config.readonly ? "" : `<th>Actions</th>`
        ].join("");
        const columnCount = config.columns.length + (config.readonly ? 1 : 2);
        const body = rows.length === 0
            ? `<tr><td colspan="${columnCount}" class="mail-empty">No ${this.escape(config.title)} rows found.</td></tr>`
            : rows.map(row => this.rowHtml(config, row, scope, context)).join("");

        return `<table class="mail-table" data-scope="${scope}">` +
            `<thead><tr>${headers}</tr></thead>` +
            `<tbody>${body}</tbody>` +
            `</table>`;
    },

    rowHtml: function (config, row, scope, context) {
        const rowKey = this.rowKey(config, row);
        const values = config.columns
            .map(column => `<td>${this.escape(this.displayValue(row[column]))}</td>`)
            .join("");
        const expandButton = this.canExpand(config, row)
            ? `<button data-action="toggle" data-scope="${scope}" data-key="${this.escape(rowKey)}" type="button">+</button>`
            : "";

        this.storeRow(scope, rowKey, row, context);

        const actions = config.readonly
            ? ""
            : `<td class="mail-actions">` +
                `<button data-action="edit" data-scope="${scope}" data-key="${this.escape(rowKey)}" type="button">Edit</button>` +
                `<button data-action="delete" data-scope="${scope}" data-key="${this.escape(rowKey)}" type="button">Delete</button>` +
                `</td>`;

        return `<tr data-row-key="${this.escape(rowKey)}">` +
            `<td class="mail-expand-column">${expandButton}</td>` +
            values +
            actions +
            `</tr>`;
    },

    bindGridActions: function (container) {
        container.querySelectorAll("[data-action]").forEach(button =>
            button.addEventListener("click", event => this.onAction(event)));
    },

    storeRow: function (scope, key, row, context) {
        this.store[scope] = this.store[scope] || {};
        this.store[scope][key] = { row, context };
    },

    stored: function (scope, key) {
        return this.store[scope]?.[key] ?? null;
    },

    onAction: async function (event) {
        const button = event.currentTarget;
        const action = button.dataset.action;
        const stored = this.stored(button.dataset.scope, button.dataset.key);

        if (!stored) {
            return;
        }

        if (action === "toggle") {
            await this.toggleDetails(button, stored);
            return;
        }

        if (action === "edit") {
            this.openEditor(this.configForScope(button.dataset.scope), stored.row, stored.context);
            return;
        }

        if (action === "delete") {
            await this.deleteRow(this.configForScope(button.dataset.scope), stored.row);
        }
    },

    toggleDetails: async function (button, stored) {
        const row = button.closest("tr");
        const existing = row.nextElementSibling;

        if (existing?.classList.contains("mail-detail-row")) {
            existing.remove();
            button.textContent = "+";
            return;
        }

        button.textContent = "-";
        const detailRow = document.createElement("tr");
        detailRow.className = "mail-detail-row";
        detailRow.innerHTML = `<td colspan="${row.children.length}"></td>`;
        row.after(detailRow);

        const config = this.configForScope(button.dataset.scope);

        if (config.name === "MailServer") {
            detailRow.querySelector("td").innerHTML = this.detailShellHtml();
            await this.loadMailServerDetails(detailRow, stored.row);
            return;
        }

        if (config.name === "QueuedEmail") {
            this.loadQueuedEmailDetails(detailRow, stored.row);
        }
    },

    canExpand: function (config, row) {
        return config.name === "MailServer"
            || (config.name === "QueuedEmail" && Array.isArray(row.FailedSends));
    },

    detailShellHtml: function () {
        return `<div class="mail-detail">` +
            `<div class="mail-tabs">` +
            `<button class="active" data-detail-tab="queued" type="button">Queued Mail</button>` +
            `<button data-detail-tab="sent" type="button">Sent Mail</button>` +
            `</div>` +
            `<section class="mail-tab-panel active" data-detail-panel="queued">` +
            `<div class="mail-detail-toolbar"><button data-create-child="QueuedEmail" type="button">Create Queued Email</button></div>` +
            `<div data-child-grid="QueuedEmail"></div>` +
            `</section>` +
            `<section class="mail-tab-panel" data-detail-panel="sent">` +
            `<div class="mail-detail-toolbar"><button data-create-child="SentEmail" type="button">Create Sent Email</button></div>` +
            `<div data-child-grid="SentEmail"></div>` +
            `</section>` +
            `</div>`;
    },

    loadMailServerDetails: async function (detailRow, mailServer) {
        const context = { mailServer };

        detailRow.querySelectorAll("[data-detail-tab]").forEach(tab =>
            tab.addEventListener("click", () => this.showDetailTab(detailRow, tab.dataset.detailTab)));

        detailRow.querySelectorAll("[data-create-child]").forEach(button =>
            button.addEventListener("click", () => {
                const config = this.configs[button.dataset.createChild];
                this.openEditor(config, null, context, () => this.loadMailServerDetails(detailRow, mailServer));
            }));

        await this.renderChildGrid(detailRow, this.configs.QueuedEmail, context);
        await this.renderChildGrid(detailRow, this.configs.SentEmail, context);
    },

    renderChildGrid: async function (container, config, context) {
        const grid = container.querySelector(`[data-child-grid='${config.name}']`);
        const rows = await this.read(config, context);
        const scope = `${config.name}-${context.mailServer.Id}`;
        grid.innerHTML = this.tableHtml(config, rows, scope, context);
        this.bindGridActions(grid);
    },

    loadQueuedEmailDetails: function (detailRow, queuedEmail) {
        const failures = queuedEmail.FailedSends ?? [];
        detailRow.querySelector("td").innerHTML =
            `<div class="mail-detail">` +
            `<div class="mail-detail-toolbar"><strong>Send Failures</strong></div>` +
            `<div data-child-grid="EmailSendFailure"></div>` +
            `</div>`;

        const grid = detailRow.querySelector("[data-child-grid='EmailSendFailure']");
        grid.innerHTML = this.tableHtml(
            this.configs.EmailSendFailure,
            failures,
            `EmailSendFailure-${queuedEmail.Id}`,
            { queuedEmail });
    },

    showDetailTab: function (detailRow, tabName) {
        detailRow.querySelectorAll("[data-detail-tab]").forEach(tab =>
            tab.classList.toggle("active", tab.dataset.detailTab === tabName));
        detailRow.querySelectorAll("[data-detail-panel]").forEach(panel =>
            panel.classList.toggle("active", panel.dataset.detailPanel === tabName));
    },

    configForScope: function (scope) {
        const name = scope.split("-")[0];
        return this.configs[name];
    },

    openEditor: function (config, row, context, afterSave) {
        const dialog = document.getElementById("editor-dialog");
        const fields = document.getElementById("editor-fields");
        document.getElementById("editor-title").textContent = `${row ? "Edit" : "Create"} ${config.title}`;
        fields.innerHTML = Object.entries(config.fields)
            .filter(([, field]) => row || field.create !== false)
            .map(([name, field]) => this.fieldHtml(name, field, row, context, config))
            .join("");

        const form = dialog.querySelector("form");
        form.onsubmit = async event => {
            event.preventDefault();

            if (event.submitter?.id === "editor-close") {
                dialog.close();
                return;
            }

            await this.saveEditor(config, row, context);
            dialog.close();

            if (afterSave) {
                await afterSave();
            } else {
                await this.loadAll();
            }
        };

        dialog.showModal();
    },

    fieldHtml: function (name, field, row, context, config) {
        const value = this.contextValue(name, context, config) ?? row?.[name] ?? this.defaultValue(name, field);
        const readonly = field.readonly || this.contextValue(name, context, config) !== null ? "readonly" : "";

        if (field.type === "checkbox") {
            const checked = value === true || value === "true" ? "checked" : "";
            return `<label><span>${this.escape(field.label)}</span><input name="${name}" type="checkbox" ${checked} ${readonly}></label>`;
        }

        const input = field.type === "textarea"
            ? `<textarea name="${name}" ${readonly}>${this.escape(value)}</textarea>`
            : `<input name="${name}" value="${this.escape(value)}" ${field.type === "password" ? "type=\"password\"" : ""} ${readonly}>`;

        return `<label><span>${this.escape(field.label)}</span>${input}</label>`;
    },

    saveEditor: async function (config, row, context) {
        const data = this.editorPayload(config, context);

        if (row) {
            await MailApi.put(`${this.apiRoot}/${config.name}(${row[config.key]})`, data);
            MailApi.notify(`${config.title} updated`);
            return;
        }

        await MailApi.post(`${this.apiRoot}/${config.name}`, data);
        MailApi.notify(`${config.title} created`);
    },

    editorPayload: function (config, context) {
        const form = document.getElementById("editor-fields");
        const payload = {};

        Object.entries(config.fields).forEach(([name, field]) => {
            const input = form.querySelector(`[name='${name}']`);

            if (!input || field.create === false && input.value === "") {
                return;
            }

            payload[name] = this.coerceInput(input, field);
        });

        if (context?.mailServer) {
            payload.AppId = context.mailServer.AppId;

            if (config.name === "QueuedEmail") {
                payload.MailServerName = context.mailServer.Name;
            }
        }

        if ((config.name === "QueuedEmail" || config.name === "SentEmail") && !payload.SentByUserId) {
            payload.SentByUserId = MailApi.currentUserId();
        }

        if (config.name === "SentEmail" && !payload.SentOn) {
            payload.SentOn = new Date().toISOString();
        }

        return payload;
    },

    deleteRow: async function (config, row) {
        if (!confirm(`Delete ${config.title}?`)) {
            return;
        }

        await MailApi.delete(`${this.apiRoot}/${config.name}(${row[config.key]})`);
        MailApi.notify(`${config.title} deleted`);
        await this.loadAll();
    },

    contextValue: function (name, context, config) {
        if (!context?.mailServer) {
            return null;
        }

        if (name === "AppId" && (config.name === "QueuedEmail" || config.name === "SentEmail")) {
            return context.mailServer.AppId;
        }

        if (name === "MailServerName" && config.name === "QueuedEmail") {
            return context.mailServer.Name;
        }

        return null;
    },

    showMainTab: function (tabName) {
        document.querySelectorAll("[data-main-tab]").forEach(tab =>
            tab.classList.toggle("active", tab.dataset.mainTab === tabName));
        document.querySelectorAll("[data-main-panel]").forEach(panel =>
            panel.classList.toggle("active", panel.dataset.mainPanel === tabName));
    },

    rowKey: function (config, row) {
        return row[config.key];
    },

    label: function (config, column) {
        return config.fields[column]?.label ?? column;
    },

    defaultValue: function (name, field) {
        if (field.type === "number") {
            return name === "Port" ? 587 : 0;
        }

        if (field.type === "checkbox") {
            return true;
        }

        if (name === "SentByUserId") {
            return MailApi.currentUserId();
        }

        return "";
    },

    coerceInput: function (input, field) {
        if (field.type === "checkbox") {
            return input.checked;
        }

        if (field.type === "number") {
            return Number(input.value || 0);
        }

        return input.value;
    },

    displayValue: function (value) {
        if (value === null || value === undefined) {
            return "";
        }

        if (typeof value === "string" && value.length > 80) {
            return `${value.substring(0, 77)}...`;
        }

        return value;
    },

    dateInputValue: function (id) {
        const value = document.getElementById(id)?.value;
        return value ? new Date(value).toISOString() : null;
    },

    odataString: function (value) {
        return String(value ?? "").replace(/'/g, "''");
    },

    escape: function (value) {
        return String(value ?? "")
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;");
    }
};

document.addEventListener("mail-auth-changed", event => {
    if (event.detail.isAuthenticated) {
        window.MailGrids.init();
    }
});

document.addEventListener("DOMContentLoaded", () => window.MailGrids.init());
