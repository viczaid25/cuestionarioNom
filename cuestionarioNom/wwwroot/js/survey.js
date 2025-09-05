async function postAnswer(surveyId, questionId, optionId) {
    try {
        await fetch('/Surveys/Answer', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: `surveyId=${encodeURIComponent(surveyId)}&questionId=${encodeURIComponent(questionId)}&selectedOptionId=${encodeURIComponent(optionId)}`
        });
    } catch (e) {
        console.error('Error saving answer', e);
        alert('No se pudo guardar la respuesta. Reintenta.');
    }
}
