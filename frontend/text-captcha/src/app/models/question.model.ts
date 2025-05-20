
export interface CreateOptionDTO{
    optionText: string;
    isCorrect: boolean;
}


export interface CreateQuestionDTO {
    questionText: string;
    options: CreateOptionDTO[];
}

