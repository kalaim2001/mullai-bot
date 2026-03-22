export function initialize(element, dotNetHelper) {
    if (!element) return;

    element.addEventListener('keydown', (e) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            dotNetHelper.invokeMethodAsync('OnEnterPressed');
        }
    });
}
