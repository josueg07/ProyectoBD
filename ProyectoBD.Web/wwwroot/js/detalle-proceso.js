(() => {
    const moneda = new Intl.NumberFormat('es-GT', { style: 'currency', currency: 'GTQ' });
    document.querySelectorAll('[data-detalle-proceso]').forEach(form => {
        const recalcular = () => {
            let total = 0;
            form.querySelectorAll('[data-linea]').forEach(row => {
                const cantidad = Number(row.querySelector('[data-cantidad]')?.value || 0);
                const precio = Number(row.querySelector('input[data-precio]')?.value || 0);
                const subtotal = cantidad > 0 && precio > 0 && Number.isFinite(cantidad * precio) ? cantidad * precio : 0;
                total += subtotal;
                const celda = row.querySelector('[data-subtotal]');
                if (celda) celda.textContent = moneda.format(subtotal);
            });
            const salida = form.querySelector('[data-total]');
            if (salida) salida.textContent = moneda.format(total);
        };
        form.querySelectorAll('[data-variante]').forEach(select => {
            select.addEventListener('change', () => {
                const precio = select.closest('tr').querySelector('input[data-precio]');
                if (precio && select.value) precio.value = select.selectedOptions[0].dataset.precio;
                recalcular();
            });
        });
        form.addEventListener('input', recalcular);
        recalcular();
    });
    document.querySelectorAll('[data-recepcion]').forEach(form => {
        const actualizar = () => form.querySelectorAll('[data-recepcion-linea]').forEach(row => {
            const cantidad = row.querySelector('[data-recibir]');
            const costo = row.querySelector('[data-costo]');
            costo.required = Number(cantidad.value) > 0;
            costo.min = costo.required ? '0.01' : '0';
        });
        form.querySelector('[data-recibir-todo]')?.addEventListener('click', () => {
            form.querySelectorAll('[data-recibir]').forEach(input => input.value = input.dataset.pendiente);
            actualizar();
        });
        form.addEventListener('input', actualizar);
        actualizar();
    });
})();
