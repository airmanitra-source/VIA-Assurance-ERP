/**
 * Load and preview Word documents using Mammoth.js
 */
window.loadWordDocument = async function(apiUrl, containerId) {
    const container = document.getElementById(containerId);
    if (!container) {
        console.error(`Container ${containerId} not found`);
        return;
    }

    try {
        // Show loading indicator
        container.innerHTML = '<div style="padding: 20px; text-align: center;">Loading document...</div>';

        // Fetch the Word document from the API
        const response = await fetch(apiUrl);
        if (!response.ok) {
            throw new Error(`Failed to fetch document: ${response.statusText}`);
        }

        // Get the document as an array buffer
        const arrayBuffer = await response.arrayBuffer();

        // Convert DOCX to HTML using Mammoth
        const result = await mammoth.convertToHtml({ arrayBuffer: arrayBuffer });

        // Insert the converted HTML into the container
        container.innerHTML = result.value;

        // Log any warnings
        if (result.messages.length > 0) {
            console.warn('Word document conversion warnings:', result.messages);
        }

        // Apply styling to the converted content
        const wordContent = container.querySelector('*');
        if (wordContent) {
            wordContent.style.fontFamily = 'Calibri, Arial, sans-serif';
            wordContent.style.lineHeight = '1.5';
        }
    } catch (error) {
        console.error('Error loading Word document:', error);
        container.innerHTML = `<div style="padding: 20px; color: #dc3545;">
            <p>Error loading document:</p>
            <p>${error.message}</p>
        </div>`;
    }
};
