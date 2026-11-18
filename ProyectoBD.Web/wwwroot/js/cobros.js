document.querySelectorAll('[data-cobro]').forEach(form => {
    const actualizar = () => {
        const total = Array.from(form.querySelectorAll('[data-monto]')).reduce((suma, input) => suma + (Number(input.value) || 0), 0);
        form.querySelector('[data-total-pagos]').textContent = new Intl.NumberFormat('es-GT', { style: 'currency', currency: 'GTQ' }).format(total);
    };
    form.addEventListener('input', actualizar);
    actualizar();
});
